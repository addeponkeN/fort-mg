using Microsoft.Xna.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fort.MG.YamlConverters;

public class ColorYamlConverter : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(Color);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
	{
		var scalar = parser.Consume<Scalar>().Value;
		var parts = scalar.Split(',');
		return new Color(
			byte.Parse(parts[0]),
			byte.Parse(parts[1]),
			byte.Parse(parts[2]),
			byte.Parse(parts[3])
		);
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
	{
		var c = (Color)value!;
		emitter.Emit(new Scalar($"{c.R},{c.G},{c.B},{c.A}"));
	}
}
