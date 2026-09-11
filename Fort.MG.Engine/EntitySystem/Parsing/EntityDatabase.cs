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

    /// <summary>
    /// Names (lower-case file name without extension) of every mapped entity template. Scans the
    /// templates folder on first use. Additive: the mapping was previously only built lazily by
    /// <see cref="GetFile"/> in DEBUG builds.
    /// </summary>
    public static IReadOnlyCollection<string> GetAllTemplateNames()
    {
        if (MappedFiles.Count == 0)
            MapFiles();

        return MappedFiles.Keys;
    }

    /// <summary>
    /// Absolute path of the template file backing <paramref name="name"/>, or null when unknown.
    /// Additive: lets the editor save edits back to the exact file a template was loaded from.
    /// </summary>
    public static string? GetTemplatePath(string name)
    {
        if (MappedFiles.Count == 0)
            MapFiles();

        return MappedFiles.TryGetValue(name.ToLowerInvariant(), out var path) ? path : null;
    }
}