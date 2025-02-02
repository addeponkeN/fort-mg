using Fort.MG.Assets.Data;

namespace Fort.MG.Assets.Storage;

public class SpriteAtlasStorage : BaseStorage<SpriteAtlas>
{
	public override object Load(string name)
	{
		var atlas = AssetManager.ContentManager.Load<SpriteAtlas>(name);
		Add(name, atlas);
		return atlas;
	}
}