using FontStashSharp;
using Fort.MG.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class Label : GuiComponent
{
	private readonly TextRenderer _textRenderer = new();

	public DynamicSpriteFont Font
	{
		get => _textRenderer.Font;
		set
		{
			_textRenderer.Font = value;
			IsPositionDirty = true;
		}
	}

	public string Text
	{
		get => _textRenderer.Text;
		set
		{
			if (_textRenderer.Text.Equals(value))
				return;
			_textRenderer.Text = value;
			UpdateSize();
			IsPositionDirty = true;
		}
	}

	public override Vector2 Position
	{
		get => _textRenderer.Position;
		set
		{
			_textRenderer.Position = value;
			IsPositionDirty = true;
		}
	}

	public override Color Foreground
	{
		get => _textRenderer.Color;
		set => _textRenderer.Color = value;
	}

	private void UpdateSize()
	{
		if (_textRenderer.Font != null)
			Size = _textRenderer.Font.MeasureString(_textRenderer.Text);
	}

	public override void DrawText()
	{
		base.DrawText();
		UpdateTransforms();
		_textRenderer.DrawText(_canvas.Sb);

		Bounds.DrawLined(Color.MonoGameOrange);
	}
}

public class TextRenderer
{
	public DynamicSpriteFont Font = GuiContent.GetDefaultFont();
	public Vector2 Position;
	public Color Color = Color.White;
	public string Text = "";

	public void DrawText() => DrawText(Graphics.SpriteBatch);

	public void DrawText(SpriteBatch sb)
	{
		Font.DrawText(sb, Text, Position, Color, 0f, Vector2.Zero, Vector2.One, 0f, 0f, 0f, TextStyle.None, FontSystemEffect.None, 0);
	}
}
