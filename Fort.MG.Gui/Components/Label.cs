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

    public bool TextShadow
    {
        get => _textRenderer.Shadow;
        set => _textRenderer.Shadow = value;
    }

    public override Color Foreground
    {
        get => base.Foreground * Style.Opacity;
        set => base.Foreground = value;
    }

    private void UpdateSize()
    {
        Size = _textRenderer.Font.MeasureString(_textRenderer.Text);
        UpdateTransforms();
    }

    public override void DrawText()
    {
        base.DrawText();
        _textRenderer.Transform = Canvas?.TransformMatrix ?? Canvas.DefaultScaleTransform;
        _textRenderer.Color = Foreground;
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
    private Vector2 _position;
    private Vector2 _drawPosition;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _drawPosition = Vector2.Transform(_position, Transform);
        }
    }

    private Vector2 _scale = Vector2.One;
    public Color Color = Color.White;
    public string Text = "";
    public bool Shadow = false;
    private Matrix _transform = Matrix.Identity;

    internal Matrix Transform
    {
        get => _transform;
        set
        {
            if (value.Equals(_transform))
                return;

            _transform = value;
            _scale = _transform.Decompose(out var scale3, out _, out _)
                ? new Vector2(scale3.X, scale3.Y)
                : Vector2.One;
            _drawPosition = Vector2.Transform(_position, Transform);
        }
    }

    public void DrawText() => DrawText(Graphics.SpriteBatch);

    public void DrawText(SpriteBatch sb)
    {
        if (Shadow)
        {
            DrawTextShadow(sb);
        }


        Font.DrawText(sb, Text, _drawPosition, Color, 0f, Vector2.Zero, _scale, 0f, 0f, 0f, TextStyle.None,
            FontSystemEffect.None, 0);
    }

    public void DrawTextShadow(SpriteBatch sb)
    {
        var clr = Color.Black * (Color.A / 255f);
        Font.DrawText(sb, Text, _drawPosition + Vector2.One, clr, 0f, Vector2.Zero,
            _scale, 0f,
            0f, 0f,
            TextStyle.None, FontSystemEffect.None, 0);
    }

    public static void Draw(string text, Vector2 pos)
    {
        var font = GuiContent.GetDefaultFont(16);
        font.DrawText(Graphics.SpriteBatch, text, pos, Color.White);
    }

    public Vector2 GetSize()
    {
        return new Vector2(Font.MeasureString(Text).X + 1, Font.FontSize - 6);
    }
}