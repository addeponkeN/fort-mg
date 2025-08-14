using Fort.MG.Assets.Data;

namespace Fort.MG.Assets.Storage;

public class SpriteAtlasStorage() : BaseStorage<SpriteAtlas>("textures")
{
	public override SpriteAtlas Load(string name)
	{
		var path = GetFilePath(name);
		var atlas = AssetManager.ContentManager.Load<SpriteAtlas>(path);
		Add(name, atlas);
		return atlas;
	}

	public override SpriteAtlas Get(string name)
	{
		if (!Storage.TryGetValue(name, out var asset))
		{
			asset = Load(name);
		}

		return asset;
	}
}