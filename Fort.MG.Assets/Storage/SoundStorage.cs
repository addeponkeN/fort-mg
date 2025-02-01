using Microsoft.Xna.Framework.Audio;

namespace Fort.MG.Assets.Storage;

public class SoundStorage : BaseStorage<SoundEffect>
{
    public override object Load(string name)
    {
        var s = AssetManager.ContentManager.Load<SoundEffect>(GetFilePath(name));
        Add(name, s);
        return s;
    }
}