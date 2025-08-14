using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Fort.MG.Assets.Storage;

public class FontStorage() : BaseStorage<FontSystem>("fonts")
{
    public override FontSystem Load(string name)
    {
        var f = new FontSystem();
        f.AddFont(TitleContainer.OpenStream(GetFilePath(name)));
        Add(name, f);
        return f;
    }
}