using Fort.MG.Assets.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fort.MG.JsonConverters;

public class SpriteRegionConverter : JsonConverter<SpriteRegion>
{
	public override SpriteRegion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var json = JsonDocument.ParseValue(ref reader).RootElement;
		var atlasName ="world";// json.GetProperty("atlasName").GetString();
		var name = json.GetProperty("name").GetString();

		var atlas = FortEngine.Assets.GetAsset<SpriteAtlas>(atlasName);
		if (atlas?.Regions.FirstOrDefault(r => r.Name == name) is SpriteRegion region)
			return region;

		var def = atlas.Regions.FirstOrDefault(x => x.Name == "pixel");
		return new SpriteRegion(name, def, def);
	}

	public override void Write(Utf8JsonWriter writer, SpriteRegion value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		//writer.WriteString("atlasName", value.AtlasName);
		writer.WriteString("name", value.Name);
		writer.WriteEndObject();
	}
}