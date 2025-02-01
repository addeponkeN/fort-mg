using System.IO;
using System.Text.Json;
using Fort.MG.Assets.Data;

namespace Fort.MG.Assets.Storage;

public class SpriteAtlasStorage : BaseStorage<SpriteAtlas>
{
	private readonly TextureStorage _texStorage;

	public SpriteAtlasStorage(TextureStorage texStorage)
	{
		_texStorage = texStorage;
	}

	public override object Load(string name)
	{
		var filePath = GetFilePath(name);
		_texStorage.Load(filePath);
		var json = File.ReadAllText(filePath);
		var atlas = JsonSerializer.Deserialize<SpriteAtlas>(json);
		Add(name, atlas);
		return atlas;
	}
}