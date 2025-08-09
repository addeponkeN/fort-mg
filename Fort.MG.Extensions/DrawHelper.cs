using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class DrawHelper
{
	internal static SpriteBatch Sb;
	internal static Texture2D Pixel;
	internal static Rectangle PixelSource;

	/// <summary>
	/// Draws a straight line between two points.
	/// </summary>
	public static void DrawLine(Vector2 start, Vector2 end, Color color, float size)
	{
		Vector2 edge = end - start;
		float angle = (float)Math.Atan2(edge.Y, edge.X);
		float length = edge.Length();
		Vector2 origin = new Vector2(0f, 0.5f);
		Sb.Draw(Pixel, start, PixelSource, color, angle, origin, new Vector2(length, size), SpriteEffects.None, 0);
	}

	public static void DrawRec(Vector2 pos, Vector2 size, Color clr)
	{
		Sb.Draw(Pixel, pos, PixelSource, clr, 0f, Vector2.Zero, size, SpriteEffects.None, 0f);
	}

	public static void DrawRec(this Rectangle rect, Color color)
	{
		Sb.Draw(Pixel, rect, PixelSource, color);
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
