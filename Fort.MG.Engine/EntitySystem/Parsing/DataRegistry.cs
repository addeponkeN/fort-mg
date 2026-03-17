using Fort.MG.Assets;
using Fort.MG.YamlConverters;
using YamlDotNet.Serialization;

namespace Fort.MG.EntitySystem.Parsing;

public abstract class DataObject
{
    public int Version { get; set; }
    public string Name { get; set; }
}

public static class DataRegistry
{
    private static readonly Dictionary<string, DataObject> _objects = new();

    public static T Get<T>(string name) where T : DataObject
    {
        if (_objects.TryGetValue(name, out var obj))
        {
            if (obj is T t)
                return t;

            throw new InvalidOperationException(
                $"DataObject '{name}' exists but is of type {obj.GetType().Name}, not {typeof(T).Name}.");
        }

        return DataLoader.Load<T>(name);
    }

    public static DataObject Get(Type type, string name)
    {
        if (_objects.TryGetValue(name, out var obj))
        {
            if (obj.GetType() == type)
                return obj;

            throw new InvalidOperationException(
                $"DataObject '{name}' exists but is of type {obj.GetType().Name}, not {type.Name}.");
        }

        return DataLoader.Load(type, name);
    }

    public static void Add(DataObject obj)
    {
        _objects[obj.Name] = obj;
    }
}

public static class DataLoader
{
    public static string DataFolderPath { get; set; } = "dataobjects";

    private static readonly IDeserializer Deserializer = YamlSerializationFactory.DataObjectDeserializer;

    private static Dictionary<string, string> _map = new();

    static DataLoader()
    {
        MapFiles();
    }

    private static void MapFiles()
    {
        var folder = Path.Combine(AssetManager.ContentRoot, DataFolderPath);
        if (!Directory.Exists(folder))
        {
            Logger.Error($"Missing 'dataobjects' folder: {folder}");
            return;
        }

        _map.Clear();

        var files = Directory.GetFiles(folder, "*.yaml", SearchOption.AllDirectories);

        Logger.Info($"# Mapping dataobjects ({files.Length}) #");

        const string basePath = "content\\dataobjects\\";
        const string yamlExt = ".yaml";

        foreach (var f in files)
        {
            string trimmed = f[basePath.Length..^yamlExt.Length];
            var fileName = trimmed.ToLowerInvariant().Replace('\\', '/');

            _map.Add(fileName, f);
            Logger.Info($" - {fileName} ({f})");
        }
    }

    private static string GetFile(string name)
    {
        name = name.ToLowerInvariant();

        string path = string.Empty;

        string query = $"{name}";
        if (_map.TryGetValue(query, out path))
            return path;

#if DEBUG
        // try to remap files
        MapFiles();

        if (!_map.TryGetValue(name, out path))
            Logger.Error($"Dataobject not found: {name}");
#endif

        return path;
    }

    public static T Load<T>(string name) where T : DataObject
    {
        var path = GetFile(name);
        var yaml = File.ReadAllText(path);

        var obj = Deserializer.Deserialize<T>(yaml);

        if (obj.Name == null)
            Logger.Warn($"DataObject '{name}' missing Name field");

        obj.Name ??= name;

        DataRegistry.Add(obj);
        return obj;
    }

    public static DataObject Load(Type type, string name)
    {
        var path = GetFile(name);
        var yaml = File.ReadAllText(path);
        DataObject obj;
        try
        {
            DataObjectReferenceConverter.IsLoadingDataObject = true;
            obj = (DataObject)Deserializer.Deserialize(yaml, type);
        }
        finally
        {
            DataObjectReferenceConverter.IsLoadingDataObject = false;
        }

        if (obj.Name == null)
            Logger.Warn($"DataObject '{name}' missing Name field");

        obj.Name ??= name;

        DataRegistry.Add(obj);
        return obj;
    }

}
