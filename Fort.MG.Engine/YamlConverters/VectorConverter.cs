using Microsoft.Xna.Framework;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fort.MG.YamlConverters;

public class Vector2YamlConverter : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(Vector2);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
	{
		var scalar = parser.Consume<Scalar>().Value;
		var parts = scalar.Split(',');
		return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
	{
		var v = (Vector2)value!;
		emitter.Emit(new Scalar($"{v.X},{v.Y}"));
	}
}


public class Vector3YamlConverter : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(Vector3);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
	{
		var scalar = parser.Consume<Scalar>().Value;
		var parts = scalar.Split(',');
		return new Vector3(
			float.Parse(parts[0]),
			float.Parse(parts[1]),
			float.Parse(parts[2])
		);
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
	{
		var v = (Vector3)value!;
		emitter.Emit(new Scalar($"{v.X},{v.Y},{v.Z}"));
	}
}