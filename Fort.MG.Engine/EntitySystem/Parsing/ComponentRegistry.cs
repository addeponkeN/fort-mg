namespace Fort.MG.EntitySystem.Parsing;

public static class ComponentRegistry
{
	private static readonly Dictionary<string, Type> ComponentTypes = new();
	private static readonly Dictionary<Type, string> TypeNames = new();

	static ComponentRegistry()
	{
		//RegisterComponent<MyCustomComponent>();
	}

	public static void RegisterComponent<T>() where T : Component
	{
		var type = typeof(T);
		var name = type.Name;
		ComponentTypes[name] = type;
		TypeNames[type] = name;
	}

	public static Type GetComponentType(string typeName)
	{
		ComponentTypes.TryGetValue(typeName, out var type);
		return type;
	}

	public static string GetTypeName(Type type)
	{
		TypeNames.TryGetValue(type, out var name);
		return name ?? type.Name;
	}

	public static Component CreateComponent(string typeName)
	{
		var type = GetComponentType(typeName);
		if (type == null) return null;

		return (Component)Activator.CreateInstance(type);
	}
}