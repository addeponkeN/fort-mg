using FontStashSharp;
using Fort.MG.Assets.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets;

public class AssetConfig
{
    public SpriteRegion Pixel;
    public FontSystem DefaultFont;

    public AssetConfig() { }
    public AssetConfig(SpriteRegion pixel, FontSystem defaultFont)
    {
        this.Pixel = pixel;
        this.DefaultFont = defaultFont;
    }
    public AssetConfig(Texture2D pixelTexture, Point pixelSourceLocation, FontSystem defaultFont)
        : this(new SpriteRegion("pixel", pixelTexture, new Rectangle(pixelSourceLocation, new Point(1))), defaultFont)
    {
        this.DefaultFont = defaultFont;
    }
    
}