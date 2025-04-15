using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class Image : GuiComponent
{
	public virtual Texture2D Texture { get; set; } = GuiContent.Pixel;
	public Rectangle Source { get; set; } = GuiContent.Pixel.Bounds;

	public override void Draw()
	{
		if (!IsVisible) return;
		if (Texture == null) return;

		Draw(Texture, Style.Foreground);
		base.DrawSkins();
		base.DrawComponents();
	}

	internal override void Draw(Texture2D tx, Color clr)
	{
		if (!IsVisible) return;
		if (tx == null) return;

		UpdateTransforms();

		var sb = Graphics.SpriteBatch;
		var scale = Size / new Vector2(Source.Width, Source.Height);
		sb.Draw(tx, Position, Source, clr, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
	}
}
