using FontStashSharp;
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
			IsDirty = true;
			UpdateSize();
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
			IsDirty = true;
		}
	}

	public override Vector2 Position
	{
		get => _textRenderer.Position;
		set
		{
			base.Position = value;
			IsDirty = true;
			UpdateTransforms();
			_textRenderer.Position = base.Position + LocalPosition;
		}
	}

	public override Color Foreground
	{
		get => _textRenderer.Color;
		set => _textRenderer.Color = value;
	}

	private void UpdateSize()
	{
		Size = _textRenderer.Font.MeasureString(_textRenderer.Text);
		UpdateTransforms();
	}

	public override void DrawText()
	{
		base.DrawText();
		_textRenderer.DrawText();
	}

	public override void DrawDebug()
	{
		base.DrawDebug();
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

	public static void Draw(string text, Vector2 pos)
	{
		var font = GuiContent.GetDefaultFont(16);
		font.DrawText(Graphics.SpriteBatch, text, pos, Color.White);
	}

	public Vector2 GetSize()
	{
		return Font.MeasureString(Text);
	}
}
