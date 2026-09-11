using Fort.MG.EntitySystem.Parsing;

namespace Fort.MG.Editor.Fields;

/// <summary>
/// Cached, reflection-derived description of every registered component type's editable fields.
///
/// Reflection runs once per component type (on first use) and the result is cached, matching the
/// engine convention that reflection is a startup-only cost (AI_CONTEXT.md §12/§15). After that the
/// inspector only touches components through the cached <see cref="ComponentField"/> accessors, and
/// the per-frame update/draw path performs no reflection.
/// </summary>
public static class ComponentFieldModel
{
	private static readonly Dictionary<Type, ComponentField[]> FieldsByType = new();
	private static readonly Dictionary<string, ComponentField[]> FieldsByName = new(StringComparer.OrdinalIgnoreCase);

	private static readonly ComponentField[] NoFields = [];

	/// <summary>Editable fields of a component type, in serialization order.</summary>
	public static IReadOnlyList<ComponentField> GetFields(Type componentType)
	{
		if (FieldsByType.TryGetValue(componentType, out var cached))
			return cached;

		var fields = Build(componentType);
		FieldsByType[componentType] = fields;
		return fields;
	}

	/// <summary>Editable fields of a component addressed by its <see cref="ComponentRegistry"/> name.</summary>
	public static IReadOnlyList<ComponentField> GetFields(string componentName)
	{
		if (FieldsByName.TryGetValue(componentName, out var cached))
			return cached;

		var type = ComponentRegistry.GetComponentType(componentName);
		var fields = type == null ? NoFields : Build(type);
		FieldsByName[componentName] = fields;
		return fields;
	}

	/// <summary>Names of every registered component type (drives the "add component" list).</summary>
	public static IEnumerable<string> GetComponentNames() => ComponentRegistry.GetAllComponentNames();

	/// <summary>Friendly name used for display and for the template's component key.</summary>
	public static string GetComponentTypeName(Type componentType) => ComponentRegistry.GetTypeName(componentType);

	private static ComponentField[] Build(Type componentType)
	{
		var list = new List<ComponentField>();

		foreach (var member in ComponentSerializer.GetSerializableMembers(componentType))
		{
			var fieldType = ComponentSerializer.GetMemberType(member);
			if (fieldType == null)
				continue;

			list.Add(new ComponentField(member, ComponentSerializer.GetSerializationName(member), fieldType));
		}

		return list.ToArray();
	}
}
