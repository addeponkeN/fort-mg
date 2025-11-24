using Fort.MG.Extensions;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Renderers;

public sealed class CircleRenderer
{
    private static readonly TextureFactory _textureFactory = new();
    private static Texture2D? _circle;
    private static Texture2D Circle => _circle ??= _textureFactory.CreateCircle(64, Color.White);

    private Vector2[] _unitCircle;
    private int _polygons;

    public int Polygons
    {
        get => _polygons;
        set
        {
            if (value != _polygons && value > 2)
            {
                _polygons = value;
                RebuildUnitCircle();
            }
        }
    }

    public CircleRenderer(int polygons = 32)
    {
        _polygons = polygons;
        RebuildUnitCircle();
    }

    public void Draw(Vector2 pos, float radius, Color color, float thickness = 1f, float layer = 0f)
    {
        var pts = _unitCircle;
        int last = _polygons - 1;

        Vector2 prev = pos + pts[0] * radius;
        for (int i = 1; i < _polygons; i++)
        {
            Vector2 p = pos + pts[i] * radius;
            DrawHelper.DrawLine(prev, p, color, thickness);
            prev = p;
        }

        DrawHelper.DrawLine(pos + pts[last] * radius, pos + pts[0] * radius, color, thickness);
    }

    public void DrawFilled(Vector2 pos, float radius, Color color, float layer = 0f)
    {
        pos -= new Vector2(radius);
        Graphics.SpriteBatch.Draw(Circle, pos, null, color, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, layer);
    }

    private void RebuildUnitCircle()
    {
        _unitCircle = new Vector2[_polygons];

        const double twoPi = Math.PI * 2.0;
        double step = twoPi / _polygons;
        double theta = 0.0;

        for (int i = 0; i < _polygons; i++)
        {
            float x = (float)Math.Cos(theta);
            float y = (float)Math.Sin(theta);
            _unitCircle[i] = new Vector2(x, y);
            theta += step;
        }
    }
}