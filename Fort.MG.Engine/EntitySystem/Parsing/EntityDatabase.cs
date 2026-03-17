using Fort.MG.Assets;

namespace Fort.MG.EntitySystem.Parsing;

public static class EntityDatabase
{
    public static string TemplatesFolderName { get; set; } = "templates";

    private static readonly Dictionary<string, string> MappedFiles = new();

    private static readonly Dictionary<string, EntityTemplate> Cache = new();

    private static string TemplatesFolderPath => Path.Combine(AssetManager.ContentRoot, TemplatesFolderName);

    private static void MapFiles()
    {
        var path = TemplatesFolderPath;

        if (!Directory.Exists(path))
        {
            Logger.Error($"Missing templates folder: {path}");
            return;
        }

        MappedFiles.Clear();

        var files = Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories);
        Logger.Info($"# Mapping templates ({files.Length}) #");

        foreach (var f in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(f).ToLowerInvariant();
            MappedFiles[fileName] = f;

            Logger.Info($" - {fileName} ({f})");

            EntityRegistry.RegisterTemplate(fileName, f);
        }
    }

    private static string GetFile(string name)
    {
        name = name.ToLowerInvariant();

        string path = string.Empty;

        if (MappedFiles.TryGetValue(name, out path))
            return path;

#if DEBUG
        // try to remap files
        MapFiles();

        if (!MappedFiles.TryGetValue(name, out path))
            Logger.Error($"Entity template not found: {name}");
#endif

        return path;
    }

    public static Entity LoadEntity(string name)
    {
        var path = GetFile(name);
        return EntitySerializer.LoadEntityTemplate(path);
    }

    public static EntityTemplate LoadEntityTemplate(string name)
    {
        if (Cache.TryGetValue(name, out var cached))
            return cached;

        string nameNormalized = name.Replace('/', '\\');

        var path = Path.Combine(TemplatesFolderPath, $"{nameNormalized}.yaml");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Entity template not found: {nameNormalized} ({path})");

        var yaml = File.ReadAllText(path);

        var template = YamlSerializationFactory.Deserializer.Deserialize<List<EntityTemplate>>(yaml)[0];

        Cache[name] = template;

        return template;
    }

    public static Entity Instantiate(string name)
    {
        var path = GetFile(name);
        var ent = EntitySerializer.LoadEntityTemplate(path);
        Entity.Instantiate(ent);
        return ent;
    }
}