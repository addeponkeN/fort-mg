using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Fort.MG.PipelineExtension;

public class TilesetCollection
{
	public string sourceFolder { get; set; }
	[JsonIgnore] public List<Tileset> sets { get; set; } = new();
}

public class Tileset
{
	[JsonIgnore] public string name { get; set; }
	[JsonIgnore] public TextureContent texture { get; set; }
}
