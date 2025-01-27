using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using System.Text.Json;

namespace Fort.MG.Assets;

public class SpriteAtlas
{
    public string Name;
    public Texture2D Texture;
    public List<SpriteData> Sprites;

    public SpriteAtlas() { }
    public SpriteAtlas(string name, Texture2D texture, List<SpriteData> sprites)
    {
        Name = name;
        Texture = texture;
        Sprites = sprites;
    }
}

public class SpriteAtlasData
{
	public string name { get; set; }
	public List<SpriteData> sprites { get; set; }
}

public class SpriteData
{
	public string name { get; set; }

	[JsonConverter(typeof(JsonRectangleConverter))]
	public Rectangle frame { get; set; }
}

public class JsonRectangleConverter : JsonConverter<Rectangle>
{
	public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.EndObject)
			return Rectangle.Empty;

		var rec = new Rectangle();

		var split = reader.GetString().Split(',');

		rec.X = int.Parse(split[0]);
		rec.Y = int.Parse(split[1]);
		rec.Width = int.Parse(split[2]);
		rec.Height = int.Parse(split[3]);

		return rec;
	}

	public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
	{
		writer.WriteStringValue($"{value.X},{value.Y},{value.Width},{value.Height}");
	}
}