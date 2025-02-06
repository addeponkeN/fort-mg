using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets.Storage;

public class TextureStorage : BaseStorage<Texture2D>
{
    public override object Load(string name)
    {
	    var t = Texture2D.FromFile(FortCore.Game.GraphicsDevice, GetFilePath(name));
        //var t = AssetManager.ContentManager.Load<Texture2D>(GetFilePath(name));
        Add(name, t);
        return t;
    }
    public override object Get(string name)
    {
        return Storage[name];
    }
}