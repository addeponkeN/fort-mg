using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class Label : GuiComponent
{
	private readonly TextRenderer _renderer = new();

	public DynamicSpriteFont Font
	{
		get => _renderer.Font;
		set => _renderer.Font = value;
	}

	public string Text
	{
		get => _renderer.Text;
		set
		{
			_renderer.Text = value;
			UpdateSize();
		}
	}

	public override Vector2 Position
	{
		get => _renderer.Position;
		set => _renderer.Position = value;
	}

	public override Color Foreground
	{
		get => _renderer.Color;
		set => _renderer.Color = value;
	}

	private void UpdateSize()
	{
		if (_renderer.Font != null)
			Size = _renderer.Font.MeasureString(_renderer.Text);
	}

	public override void Draw()
	{
		base.Draw();
		_renderer.Draw(_canvas.Sb);
	}

}

public class TextRenderer
{
	public DynamicSpriteFont Font = GuiContent.GetDefaultFont();
	public Vector2 Position;
	public Color Color = Color.White;
	public string Text = "";

	public void Draw() => Draw(Graphics.SpriteBatch);

	public void Draw(SpriteBatch sb)
	{
		Font.DrawText(sb, Text, Position, Color, 0f, Vector2.Zero, Vector2.One, 0f, 0f, 0f, TextStyle.None, FontSystemEffect.None, 0);
	}
}
