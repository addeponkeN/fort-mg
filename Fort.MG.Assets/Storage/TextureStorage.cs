using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets.Storage;

public class TextureStorage() : BaseStorage<Texture2D>("textures")
{
	public override Texture2D Load(string name)
	{
		var t = Texture2D.FromFile(FortCore.Game.GraphicsDevice, GetFilePath(name));
		Add(name, t);
		return t;
	}
	public override Texture2D Get(string name)
	{
		if (!Storage.TryGetValue(name, out var asset))
		{
			asset = Load(name);
		}
		return asset;
	}
}