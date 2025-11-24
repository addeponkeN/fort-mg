using Fort.MG.YamlConverters;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Fort.MG.EntitySystem.Parsing;

public static class ComponentSerializer
{
	public static Dictionary<string, object> SerializeComponentToDict(Component component)
	{
		var dict = new Dictionary<string, object>
		{
			["type"] = ComponentRegistry.GetTypeName(component.GetType()),
			["enabled"] = component.Enabled
		};

		component.OnBeforeSerialize();

		foreach (var member in GetSerializableMembers(component.GetType()))
		{
			var name = GetSerializationName(member);
			var value = GetMemberValue(member, component);
			if (value != null)
				dict[name] = value;
		}

		return dict;
	}

	public static Component DeserializeComponentFromDict(Dictionary<string, object> dict, Entity entity)
	{
		if (!dict.TryGetValue("type", out var typeNameObj))
			return null;

		var typeName = typeNameObj.ToString();
		var component = ComponentRegistry.CreateComponent(typeName);
		component.Entity = entity;
		if (component == null)
			return null;

		var yamlDict = new Dictionary<string, object>(dict);
		yamlDict.Remove("type");

		var tempYaml = YamlSerializationFactory.Serializer.Serialize(yamlDict);
		var typedComponent = (Component)YamlSerializationFactory.Deserializer.Deserialize(tempYaml, component.GetType());

		foreach (var member in GetSerializableMembers(component.GetType()))
		{
			var value = GetMemberValue(member, typedComponent);
			SetMemberValue(member, component, value);
		}

		if (dict.TryGetValue("enabled", out var enabledObj))
			component.Enabled = Convert.ToBoolean(enabledObj);

		component.OnAfterDeserialize();
        return component;
	}

	private static object ConvertValue(object value, Type targetType)
	{
		if (value == null || targetType == null) return value;

		if (targetType.IsInstanceOfType(value))
			return value;

		if (TryConvertWithYamlConverters(value, targetType, out var convertedValue))
			return convertedValue;

		if (targetType.IsEnum && value is string enumString)
		{
			try
			{
				return Enum.Parse(targetType, enumString, true);
			}
			catch
			{
				// ignore
			}
		}

		try
		{
			return Convert.ChangeType(value, targetType);
		}
		catch
		{
			return value;
		}
	}

	private static bool TryConvertWithYamlConverters(object value, Type targetType, out object convertedValue)
	{
		convertedValue = null;

		var converters = YamlSerializationFactory.AllConverters;

		var converter = converters.FirstOrDefault(c => c.Accepts(targetType));
		if (converter == null) return false;

		try
		{
			var tempYaml = YamlSerializationFactory.Serializer.Serialize(value);
			using var reader = new StringReader(tempYaml);
			var parser = new YamlDotNet.Core.Parser(reader);

			parser.MoveNext(); // StreamStart
			parser.MoveNext(); // DocumentStart or first event

			convertedValue = converter.ReadYaml(parser, targetType, null);
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static IEnumerable<MemberInfo> GetSerializableMembers(Type type)
	{
		var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
							 .Where(p => p.CanRead && p.CanWrite && HasSerializeAttribute(p));

		var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
						 .Where(HasSerializeAttribute);

		return properties.Cast<MemberInfo>().Concat(fields);
	}

	private static bool HasSerializeAttribute(MemberInfo member) =>
		member.GetCustomAttribute<SerializeAttribute>() != null;

	private static string GetSerializationName(MemberInfo member)
	{
		var attr = member.GetCustomAttribute<SerializeAttribute>();
		if (!string.IsNullOrEmpty(attr?.Name))
			return attr.Name;

		var yamlMemberAttr = member.GetCustomAttribute<YamlMemberAttribute>();
		if (!string.IsNullOrEmpty(yamlMemberAttr?.Alias))
			return yamlMemberAttr.Alias;

		return CamelCaseNamingConvention.Instance.Apply(member.Name);
	}

	private static string ToCamelCase(string name)
	{
		if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
			return name;

		return char.ToLowerInvariant(name[0]) + name.Substring(1);
	}

	private static Type GetMemberType(MemberInfo member) => member switch
	{
		PropertyInfo prop => prop.PropertyType,
		FieldInfo field => field.FieldType,
		_ => null
	};

	private static object GetMemberValue(MemberInfo member, object obj) => member switch
	{
		PropertyInfo prop => prop.GetValue(obj),
		FieldInfo field => field.GetValue(obj),
		_ => null
	};

	private static void SetMemberValue(MemberInfo member, object obj, object value)
	{
		switch (member)
		{
			case PropertyInfo prop: prop.SetValue(obj, value); break;
			case FieldInfo field: field.SetValue(obj, value); break;
		}
	}
}