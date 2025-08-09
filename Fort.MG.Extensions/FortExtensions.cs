using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public class FortExtensions
{
	public static void Initialize(SpriteBatch sb)
	{
		DrawHelper.Pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
		DrawHelper.Pixel.SetData(new[] { Color.White });
		Initialize(sb, DrawHelper.Pixel, new Rectangle(1, 1, 1, 1));
	}

	public static void Initialize(SpriteBatch sb, Texture2D pixel, Rectangle pixelSource)
	{
		DrawHelper.Sb = sb;
		DrawHelper.Pixel = pixel;
		DrawHelper.PixelSource = pixelSource;
	}
}
