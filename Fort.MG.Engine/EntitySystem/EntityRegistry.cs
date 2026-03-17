namespace Fort.MG.EntitySystem;

public static class EntityRegistry
{
    private static readonly Dictionary<string, byte> NameToType = new();
    private static readonly Dictionary<byte, string> TypeToName = new();
    private static readonly Dictionary<string, string> NameToFile = new();

    private static byte _nextType = 1; // reserve 0 for "none" or invalid

    public static void RegisterTemplate(string name, string filePath)
    {
        name = name.ToLowerInvariant();
        if (NameToType.ContainsKey(name))
            return;

        var type = _nextType++;
        NameToType[name] = type;
        TypeToName[type] = name;
        NameToFile[name] = filePath;
    }

    public static byte GetTypeForName(string name)
    {
        name = name.ToLowerInvariant();
        if (NameToType.TryGetValue(name, out var t))
            return t;

        throw new Exception($"Entity name not registered: {name}");
    }

    public static string GetNameForType(byte type)
    {
        if (TypeToName.TryGetValue(type, out var name))
            return name;

        throw new Exception($"EntityType not registered: {type}");
    }

    public static string GetFileForName(string name)
    {
        name = name.ToLowerInvariant();
        if (NameToFile.TryGetValue(name, out var path))
            return path;

        throw new Exception($"Template file not registered for: {name}");
    }

    public static IEnumerable<(string name, byte type)> AllRegistered => NameToType.Select(kv => (kv.Key, kv.Value));
}