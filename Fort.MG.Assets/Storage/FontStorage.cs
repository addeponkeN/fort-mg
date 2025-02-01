using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Fort.MG.Assets.Storage;

public class FontStorage : BaseStorage<FontSystem>
{
    public override object Load(string name)
    {
        var f = new FontSystem();
        f.AddFont(TitleContainer.OpenStream(GetFilePath(name)));
        Add(name, f);
        return f;
    }
}