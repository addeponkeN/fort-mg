using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;

namespace Fort.MG.JsonConverters;

public class ColorConverter : JsonConverter<Color>
{
	public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var strClr = reader.GetString();

		if (string.IsNullOrWhiteSpace(strClr))
			return Color.Transparent;

		if (!strClr.Contains(','))
			return new Color(int.Parse(strClr), int.Parse(strClr), int.Parse(strClr), 255);

		var split = strClr.Split(',');

		int r = int.Parse(split[0]);
		int g = int.Parse(split[1]);
		int b = int.Parse(split[2]);
		int a = int.Parse(split[3]);

		return new Color(r, g, b, a);
	}

	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
	{
		var strClr = $"{value.R},{value.G},{value.B},{value.A}";
		writer.WriteStringValue(strClr);
	}
}