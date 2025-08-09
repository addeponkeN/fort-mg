using FontStashSharp;
using Fort.MG.Extensions;
using Microsoft.Xna.Framework;

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
        // base.DrawDebug();
        Bounds.DrawLined(Color.Yellow);
    }
}