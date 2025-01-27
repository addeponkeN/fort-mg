using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets;

public class ContentConfig
{
    public AtlasRegion Pixel;
    public FontSystem DefaultFont;

    public ContentConfig()
    {
    }
    public ContentConfig(AtlasRegion pixel, FontSystem defaultFont)
    {
        this.Pixel = pixel;
        this.DefaultFont = defaultFont;
    }
    public ContentConfig(Texture2D pixelTexture, Point pixelSourceLocation, FontSystem defaultFont)
        : this(new AtlasRegion(pixelTexture, new Rectangle(pixelSourceLocation, new Point(1))), defaultFont)
    {
        this.DefaultFont = defaultFont;
    }
    
}