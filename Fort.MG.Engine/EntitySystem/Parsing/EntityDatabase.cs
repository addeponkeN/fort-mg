using Fort.MG.Assets;

namespace Fort.MG.EntitySystem.Parsing;

public static class EntityDatabase
{
    public static string TemplatesFolderPath { get; set; } = "templates";

    private static readonly Dictionary<string, string> MappedFiles = new();

    private static void MapFiles()
    {

        var templatesFolder = Path.Combine(AssetManager.ContentRoot, TemplatesFolderPath);
        if (!Directory.Exists(templatesFolder))
        {
            Logger.Error($"Missing templates folder: {templatesFolder}");
            return;
        }

        MappedFiles.Clear();

        var files = Directory.GetFiles(templatesFolder, "*.yaml", SearchOption.AllDirectories);

        Logger.Info($"# Mapping templates ({files.Length}) #");

        foreach (var f in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(f).ToLowerInvariant();
            MappedFiles.Add(fileName, f);
            Logger.Info($" - {fileName} ({f})");
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

    public static Entity Instantiate(string name)
    {
        var path = GetFile(name);
        var ent = EntitySerializer.LoadEntityTemplate(path);
        Entity.Instantiate(ent);
        return ent;
    }
}