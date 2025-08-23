using Fort.MG.YamlConverters;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Fort.MG.EntitySystem.Parsing;

public static class YamlSerializationFactory
{
	private static ISerializer BuildSerializer(params IYamlTypeConverter[] converters)
	{
		var builder = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)

			;

		foreach (var converter in converters)
			builder = builder.WithTypeConverter(converter);

		builder.ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);

		return builder.Build();
	}

	private static IDeserializer BuildDeserializer(params IYamlTypeConverter[] converters)
	{
		var builder = new DeserializerBuilder()
			.WithNamingConvention(CamelCaseNamingConvention.Instance);

		foreach (var converter in converters)
			builder = builder.WithTypeConverter(converter);

		return builder.Build();
	}

	private static readonly IYamlTypeConverter[] BaseConverters =
	[
		new Vector2YamlConverter(),
		new Vector3YamlConverter(),
		new ColorYamlConverter(),
		new RectangleYamlConverter(),
		new SpriteRegionYamlConverter(),
	];

	public static IYamlTypeConverter[] AllConverters => BaseConverters.ToArray();

	public static readonly ISerializer Serializer = BuildSerializer(BaseConverters);
	public static readonly IDeserializer Deserializer = BuildDeserializer(BaseConverters);
}