using Fort.MG.Assets.Data;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.PipelineExtensionReaders;

internal class TilesetAtlasReader : ContentTypeReader<TilesetAtlas>
{
	protected override TilesetAtlas Read(ContentReader input, TilesetAtlas existingInstance)
	{
		var texture = input.ReadObject<Texture2D>();

		int regionCount = input.ReadInt32();
		var tiles = new List<TilesetRegion>();

		for (int i = 0; i < regionCount; i++)
		{
			string tileset = input.ReadString();
			int orientation = input.ReadByte();
			int variant = input.ReadByte();
			bool isSolo = input.ReadBoolean();
			int x = input.ReadUInt16();
			int y = input.ReadUInt16();
			int width = input.ReadByte();
			int height = input.ReadByte();

			orientation = isSolo ? 15 : orientation;
			var frame = new Rectangle(x, y, width, height);
			var t = new TilesetRegion(tileset, orientation, variant, frame)
			{
				IsSolo = isSolo,
			};
			tiles.Add(t);

			Console.WriteLine($"Read tile: {t.Tileset},{t.Orientation},{t.Variant} ({t.Frame})");
		}

		return new TilesetAtlas(texture, tiles);
	}
}
