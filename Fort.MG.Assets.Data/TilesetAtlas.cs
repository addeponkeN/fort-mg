using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets.Data;

public class TilesetAtlas
{
	public string Name => Texture.Name;
	public Texture2D Texture { get; }
	public List<TilesetRegion> Regions { get; }

	public TilesetAtlas(Texture2D texture, List<TilesetRegion> regions)
	{
		Texture = texture;
		Regions = regions;
	}


	public static implicit operator Texture2D(TilesetAtlas tf) => tf.Texture;
}