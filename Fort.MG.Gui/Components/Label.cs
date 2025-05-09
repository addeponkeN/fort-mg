using System.Text;
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
    private Vector2 _position;
    private Vector2 _drawPosition;
    private Vector2 _scale = Vector2.One;
    private Matrix _transform = Matrix.Identity;
    private Color[]? _characterColors;
    private Color[]? _baseCharacterColors;
    private string _parsedText = "";
    private string _rawText = "";
    private bool _isMarkdown;

    public bool Shadow = false;
    public DynamicSpriteFont Font = GuiContent.GetDefaultFont();
    private Color _color = Color.White;

    public Color Color
    {
        get => _color;
        set
        {
            if (_color.Equals(value))
                return;

            if (_isMarkdown && _characterColors != null && _baseCharacterColors != null)
            {
                for (int i = 0; i < _characterColors.Length; i++)
                {
                    if (_characterColors[i] == _color)
                        _characterColors[i] = value;
                    else
                    {
                        _characterColors[i] = _baseCharacterColors[i] * (value.A / 255f);
                    }
                }
            }

            _color = value;
        }
    }

    public string Text
    {
        get => _parsedText;
        set
        {
            if (_rawText.Equals(value))
                return;

            _rawText = value;
            ParseMarkdownText(value);
        }
    }

    public string RawText => _rawText;

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            _drawPosition = Vector2.Transform(_position, Transform);
        }
    }

    public Matrix Transform
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

    public void SetText(string text, bool parseMarkdown = true)
    {
        if (parseMarkdown)
        {
            Text = text;
        }
        else
        {
            _rawText = text;
            _parsedText = text;
            _isMarkdown = false;
        }
    }

    public Vector2 GetSize() => new(Font.MeasureString(Text).X + 1, Font.FontSize - 6);

    public void DrawText() => DrawText(Graphics.SpriteBatch);

    public void DrawText(SpriteBatch sb)
    {
        if (Shadow)
        {
            DrawTextShadow(sb);
        }

        if (_isMarkdown && _characterColors != null)
        {
            Font.DrawText(sb, _parsedText, _drawPosition, _characterColors, 0f, Vector2.Zero, _scale, 0f, 0f, 0f,
                TextStyle.None, FontSystemEffect.None, 0);
        }
        else
        {
            Font.DrawText(sb, _parsedText, _drawPosition, Color, 0f, Vector2.Zero, _scale, 0f, 0f, 0f, TextStyle.None,
                FontSystemEffect.None, 0);
        }
    }

    public void DrawTextShadow(SpriteBatch sb)
    {
        var clr = Color.Black * (Color.A / 255f);
        Font.DrawText(sb, _parsedText, _drawPosition + Vector2.One, clr, 0f, Vector2.Zero,
            _scale, 0f,
            0f, 0f,
            TextStyle.None, FontSystemEffect.None, 0);
    }

    private void ParseMarkdownText(string markdownText)
    {
        if (string.IsNullOrEmpty(markdownText))
        {
            _parsedText = "";
            _characterColors = null;
            _isMarkdown = false;
            return;
        }

        _isMarkdown = false;
        var result = new StringBuilder();
        var colors = new List<Color>();

        const char opener = '<';
        const char closer = '>';

        Color currentColor = _color;

        bool isIn = false;
        for (int i = 0; i < markdownText.Length; i++)
        {
            var c = markdownText[i];

            if (isIn)
            {
                if (c.Equals('/') && markdownText.Length > i && markdownText[i + 1] == closer)
                {
                    _isMarkdown = true;
                    isIn = false;
                    currentColor = _color;
                    i += 1;
                    continue;
                }
            }
            else
            {
                if (c.Equals(opener))
                {
                    int tagContentEnd = markdownText.IndexOf(closer, i + 2);
                    if (tagContentEnd > 0)
                    {
                        string tagContent = markdownText.Substring(i + 1, tagContentEnd - i - 1);
                        if (Markdowns.TryGetColor(tagContent, out var clr))
                        {
                            isIn = true;
                            currentColor = clr;
                            i += tagContent.Length + 1;
                            continue;
                        }
                    }
                }
            }

            result.Append(c);

            //  FontStashSharp does not want a color for whitespace, therefore text length is not equal to color length (with whitespace)
            if (!char.IsWhiteSpace(c))
                colors.Add(currentColor);
        }

        _parsedText = result.ToString();
        _characterColors = _isMarkdown ? colors.ToArray() : null;
        _baseCharacterColors = _characterColors?.ToArray();
    }
}