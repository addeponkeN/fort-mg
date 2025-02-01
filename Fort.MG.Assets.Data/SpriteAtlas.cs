using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets.Data;

public class SpriteAtlas
{
	private readonly Dictionary<string, SpriteRegion> _regionsDic;

	public string Name => Texture.Name;
	public Texture2D Texture { get; }
	public List<SpriteRegion> Regions { get; }

	public SpriteAtlas(Texture2D texture, List<SpriteRegion> regions)
	{
		Texture = texture;
		Regions = regions;
		_regionsDic = regions.ToDictionary(r => r.Name);
	}

	public SpriteRegion this[string name] => _regionsDic[name];

	public static implicit operator Texture2D(SpriteAtlas tf) => tf.Texture;

}