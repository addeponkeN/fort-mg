using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Fort.MG.PipelineExtension;

public class Atlas
{
	public string name { get; set; }
	public string sourceFolder { get; set; }
	[JsonIgnore] public List<TextureContent> textures { get; set; }
}
