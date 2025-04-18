using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions.Json;

public class PointConverter : JsonConverter<Point>
{
	public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var str = reader.GetString();
		if (string.IsNullOrWhiteSpace(str))
			throw new JsonException("Expected non-empty string for Point");

		var parts = str.Split(',');
		if (parts.Length != 2 ||
		    !int.TryParse(parts[0], out var x) ||
		    !int.TryParse(parts[1], out var y))
		{
			throw new JsonException($"Invalid Point format: '{str}'");
		}

		return new Point(x, y);
	}

	public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
	{
		writer.WriteStringValue($"{value.X},{value.Y}");
	}
}
