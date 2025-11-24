using Fort.MG.Renderers;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Extensions;

public static class DrawExtensions
{
    private static readonly CircleRenderer _circleRenderer = new();
    private static readonly TextureFactory _textureFactory = new();

    private static Texture2D? _circle;
    private static Texture2D Circle => _circle ??= _textureFactory.CreateCircle(64, Color.White);

    public static void DrawCircle(this Vector2 pos, Color clr, float radius)
    {
        _circleRenderer.Draw(pos, radius, clr);
    }

    public static void DrawCircleFilled(this Vector2 pos, Color clr, float radius)
    {
        _circleRenderer.DrawFilled(pos, radius, clr, 0.75f);
    }

}
