using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Fort.MG.JsonConverters;

public class Vector2Converter : JsonConverter<Vector2>
{
	public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var strVec = reader.GetString();
		if (string.IsNullOrWhiteSpace(strVec))
			return Vector2.Zero;

		if (!strVec.Contains(','))
			return new Vector2(float.Parse(strVec));

		var split = strVec.Split(',');

		return new Vector2(float.Parse(split[0]), float.Parse(split[1]));
	}

	public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
	{
		writer.WriteStringValue($"{value.X},{value.Y}");
	}
}

public class Vector3Converter : JsonConverter<Vector3>
{
	public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var strVec = reader.GetString();

		if (string.IsNullOrEmpty(strVec))
			return Vector3.Zero;

		if (!strVec.Contains(','))
			return new Vector3(float.Parse(strVec));

		var split = strVec.Split(',');

		int x = int.Parse(split[0]);
		int y = int.Parse(split[1]);
		int z = int.Parse(split[2]);

		return new Vector3(x, y, z);
	}

	public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
	{
		var strVec = $"{value.X},{value.Y},{value.Z}";
		writer.WriteStringValue(strVec);
	}
}