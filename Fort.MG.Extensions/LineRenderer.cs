using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Extensions;

public class LineRenderer
{
	public Texture2D Texture { get; set; } = DrawHelper.Pixel;
	public Rectangle Source { get; set; } = DrawHelper.PixelSource;

	public Vector2 Start { get; set; }
	public Vector2 End { get; set; } = new Vector2(4, 0);

	public Color Color { get; set; } = Color.White;
	public float Thickness { get; set; } = 1f;

	public void Draw() => Draw(DrawHelper.Sb);

	public void Draw(SpriteBatch sb)
	{
		Vector2 direction = End - Start;
		float length = direction.Length();
		float rotation = (float)Math.Atan2(direction.Y, direction.X);
		var scale = new Vector2(length / Source.Width, Thickness / Source.Height);

		sb.Draw(
			Texture,
			position: Start,
			sourceRectangle: Source,
			color: Color,
			rotation: rotation,
			origin: new Vector2(0f, Source.Height / 2f),
			scale: scale,
			effects: SpriteEffects.None,
			layerDepth: 0
		);
	}
}
