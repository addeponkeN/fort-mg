using System.Globalization;
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
        var parts = scalar.Split(',', 2);
        float x = float.Parse(parts[0], provider: CultureInfo.InvariantCulture);
        float y = parts.Length > 1 ? float.Parse(parts[1], provider: CultureInfo.InvariantCulture) : x;
        return new Vector2(x, y);
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
        var parts = scalar.Split(',', 3);

        float x = float.Parse(parts[0], provider: CultureInfo.InvariantCulture);
        float y = x;
        float z = x;

        if (parts.Length > 1)
        {
            y = float.Parse(parts[1], provider: CultureInfo.InvariantCulture);
            z = y;
        }

        if (parts.Length > 2)
        {
            z = float.Parse(parts[2], provider: CultureInfo.InvariantCulture);
        }

        return new Vector3(x, z, y);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var v = (Vector3)value!;
        emitter.Emit(new Scalar($"{v.X},{v.Y},{v.Z}"));
    }
}