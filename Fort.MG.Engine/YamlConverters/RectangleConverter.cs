using Microsoft.Xna.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fort.MG.YamlConverters;

public class RectangleYamlConverter : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(Rectangle);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
	{
		var scalar = parser.Consume<Scalar>().Value;
		var parts = scalar.Split(',');
		return new Rectangle(
			int.Parse(parts[0]),
			int.Parse(parts[1]),
			int.Parse(parts[2]),
			int.Parse(parts[3])
		);
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
	{
		var r = (Rectangle)value!;
		emitter.Emit(new Scalar($"{r.X},{r.Y},{r.Width},{r.Height}"));
	}
}