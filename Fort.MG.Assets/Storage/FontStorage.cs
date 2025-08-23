using FontStashSharp;
using Microsoft.Xna.Framework;

namespace Fort.MG.Assets.Storage;

public class FontStorage() : BaseStorage<FontSystem>("content\\fonts")
{
    public override FontSystem Load(string name)
    {
	    var ext = Path.GetExtension(name);
	    if (string.IsNullOrEmpty(ext))
	    {
            name += ".ttf";
		}

        var f = new FontSystem();
        var path = GetFilePath(name);
        f.AddFont(TitleContainer.OpenStream(GetFilePath(name)));
        Add(name, f);
        return f;
    }
}