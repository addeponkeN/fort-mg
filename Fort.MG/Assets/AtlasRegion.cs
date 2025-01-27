using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets;

public class AtlasRegion
{
    public Texture2D Texture;
    public Rectangle Frame;

    public AtlasRegion(Texture2D texture, Rectangle frame)
    {
        Texture = texture;
        Frame = frame;
    }

    public AtlasRegion(Texture2D texture, int x, int y, int w, int h) 
        : this(texture, new Rectangle(x, y, w, h))
    {
    }

    public static implicit operator Texture2D(AtlasRegion tf) => tf.Texture;
    public static implicit operator Rectangle(AtlasRegion tf) => tf.Frame;
}