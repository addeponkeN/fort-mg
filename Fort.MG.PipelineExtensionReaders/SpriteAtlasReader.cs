using Fort.MG.Assets.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.PipelineExtensionReaders;

/// <summary>
/// Read .xnb to memory (SpriteAtlasContent)
/// </summary>
public class SpriteAtlasReader : ContentTypeReader<SpriteAtlas>
{
	protected override SpriteAtlas Read(ContentReader input, SpriteAtlas existingInstance)
	{
		// Read the texture
		var texture = input.ReadObject<Texture2D>();

		// Read the sprite region metadata
		int regionCount = input.ReadInt32();
		var regions = new List<SpriteRegion>();

		for (int i = 0; i < regionCount; i++)
		{
			string name = input.ReadString();
			int x = input.ReadInt32();
			int y = input.ReadInt32();
			int width = input.ReadInt32();
			int height = input.ReadInt32();

			var frame = new Rectangle(x, y, width, height);
			regions.Add(new SpriteRegion(name, texture, frame));
		}

		return new SpriteAtlas(texture, regions);
	}
}