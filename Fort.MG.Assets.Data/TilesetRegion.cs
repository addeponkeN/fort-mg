using Microsoft.Xna.Framework;

namespace Fort.MG.Assets.Data;

public class TilesetRegion
{
	public string Tileset { get; }
	public Rectangle Frame { get; }

	public int TileType { get; set; }
	public int Orientation { get; }
	public int Variant { get; }

	public bool IsSolo { get; set; }

	public TilesetRegion(string tileset, int orientation, int variant, Rectangle frame)
	{
		Tileset = tileset;
		Orientation = orientation;
		Variant = variant;
		Frame = frame;
	}

	public override string ToString() => $"{Tileset} ({Orientation},{Variant}) {Frame}";
}