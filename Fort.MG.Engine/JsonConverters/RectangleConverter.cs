using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Fort.MG.JsonConverters;

public class RectangleConverter : JsonConverter<Rectangle>
{
	public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var strRect = reader.GetString();

		var split = strRect.Split(',');

		int x = int.Parse(split[0]);
		int y = int.Parse(split[1]);
		int w = int.Parse(split[2]);
		int h = int.Parse(split[3]);

		return new Rectangle(x, y, w, h);
	}

	public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
	{
		string strRect = $"{value.X},{value.Y},{value.Width},{value.Height}";
		writer.WriteStringValue(strRect);
	}
}