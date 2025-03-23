using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class DrawHelper
{
	private static SpriteBatch _sb;
	private static Texture2D _pixel;
	private static Rectangle _pixelSource;

	public static void Initialize(SpriteBatch sb)
	{
		_pixel = new Texture2D(sb.GraphicsDevice, 1, 1);
		_pixel.SetData(new[] { Color.White });
		Initialize(sb, _pixel, new Rectangle(1, 1, 1, 1));
	}

	public static void Initialize(SpriteBatch sb, Texture2D pixel, Rectangle pixelSource)
	{
		_sb = sb;
		_pixel = pixel;
		_pixelSource = pixelSource;
	}

	/// <summary>
	/// Draws a straight line between two points.
	/// </summary>
	public static void DrawLine(Vector2 start, Vector2 end, Color color, float size)
	{
		Vector2 edge = end - start;
		float angle = (float)Math.Atan2(edge.Y, edge.X);
		float length = edge.Length();
		_sb.Draw(_pixel, start, _pixelSource, color, angle, Vector2.Zero, new Vector2(length, size), SpriteEffects.None, 0);
	}

	/// <summary>
	/// Draws the border of a rectangle as a lined shape.
	/// </summary>
	public static void DrawLined(this Rectangle rectangle, Color color, float size = 1f)
	{
		Vector2 topLeft = new Vector2(rectangle.X, rectangle.Y);
		Vector2 topRight = new Vector2(rectangle.X + rectangle.Width, rectangle.Y);
		Vector2 bottomLeft = new Vector2(rectangle.X, rectangle.Y + rectangle.Height);
		Vector2 bottomRight = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);

		// Draw the four lines of the rectangle
		DrawLine(topLeft, topRight, color, size);       // Top
		DrawLine(topLeft, bottomLeft, color, size);     // Left
		DrawLine(topRight, bottomRight, color, size);   // Right
		DrawLine(bottomLeft, bottomRight, color, size); // Bottom
	}
}
