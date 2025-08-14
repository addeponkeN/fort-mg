using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using Fort.MG.Assets.Data;
using Fort.MG.JsonConverters;

namespace Fort.MG.EntitySystem.Parsing;

public static class ComponentSerializer
{
	private static readonly JsonSerializerOptions Options = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = null,
		Converters =
		{
			new Vector2Converter(),
			new Vector3Converter(),
			new ColorConverter(),
			new RectangleConverter(),
			new SpriteRegionConverter(),
		}
	};

	public static JsonNode SerializeComponent(Component component)
	{
		var componentType = component.GetType();
		var jsonObject = new JsonObject
		{
			["$type"] = ComponentRegistry.GetTypeName(componentType),
			["Enabled"] = component.Enabled
		};

		component.OnBeforeSerialize();

		// Use reflection to find serializable properties/fields
		var members = GetSerializableMembers(componentType);

		foreach (var member in members)
		{
			var value = GetMemberValue(member, component);
			var name = GetSerializationName(member);

			if (value != null)
			{
				// Serialize the value to JSON using System.Text.Json
				var jsonValue = JsonSerializer.SerializeToNode(value, Options);
				jsonObject[name] = jsonValue;
			}
		}

		return jsonObject;
	}

	public static T DeserializeComponent<T>(JsonNode json) where T : Component
	{
		return (T)DeserializeComponent(json);
	}

	public static Component DeserializeComponent(JsonNode json)
	{
		if (json is not JsonObject jsonObject)
			return null;

		var typeName = jsonObject["$type"]?.ToString();
		if (string.IsNullOrEmpty(typeName))
			return null;

		var component = ComponentRegistry.CreateComponent(typeName);
		if (component == null)
			return null;

		component.Init();

		// Set enabled state
		if (jsonObject.TryGetPropertyValue("Enabled", out var enabledNode))
		{
			component.Enabled = enabledNode.GetValue<bool>();
		}

		// Deserialize properties using reflection
		var componentType = component.GetType();
		var members = GetSerializableMembers(componentType);

		foreach (var member in members)
		{
			var name = GetSerializationName(member);
			if (jsonObject.TryGetPropertyValue(name, out var valueNode))
			{
				var memberType = GetMemberType(member);
				try
				{
					var value = valueNode.Deserialize(memberType, Options);
					SetMemberValue(member, component, value);
				}
				catch (JsonException)
				{
					// Skip invalid values
				}
			}
		}

		component.OnAfterDeserialize();
		return component;
	}

	private static IEnumerable<MemberInfo> GetSerializableMembers(Type type)
	{
		var members = new List<MemberInfo>();

		// Get properties with [Serialize] attribute
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(p => p.CanRead && p.CanWrite && HasSerializeAttribute(p));

		// Get fields with [Serialize] attribute
		var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
			.Where(f => HasSerializeAttribute(f));

		members.AddRange(properties);
		members.AddRange(fields);

		return members;
	}

	private static bool HasSerializeAttribute(MemberInfo member) =>
		member.GetCustomAttribute<SerializeAttribute>() != null;

	private static string GetSerializationName(MemberInfo member)
	{
		var attr = member.GetCustomAttribute<SerializeAttribute>();
		return attr?.Name ?? member.Name;
	}

	private static Type GetMemberType(MemberInfo member)
	{
		return member switch
		{
			PropertyInfo prop => prop.PropertyType,
			FieldInfo field => field.FieldType,
			_ => null
		};
	}

	private static object GetMemberValue(MemberInfo member, object obj)
	{
		return member switch
		{
			PropertyInfo prop => prop.GetValue(obj),
			FieldInfo field => field.GetValue(obj),
			_ => null
		};
	}

	private static void SetMemberValue(MemberInfo member, object obj, object value)
	{
		switch (member)
		{
			case PropertyInfo prop:
				prop.SetValue(obj, value);
				break;
			case FieldInfo field:
				field.SetValue(obj, value);
				break;
		}
	}
}