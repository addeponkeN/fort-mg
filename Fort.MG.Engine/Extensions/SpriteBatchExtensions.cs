using Fort.MG.Renderers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class SpriteBatchExtensions
{
    private static readonly CircleRenderer _circleRenderer = new();

    public static void DrawCircle(this SpriteBatch sb, Vector2 pos, float rad)
    {
        DrawCircle(sb, pos, rad, Color.White);
    }

    public static void DrawCircle(this SpriteBatch sb, Vector2 pos, float rad, Color color)
    {
        _circleRenderer.Draw(pos, rad, color, 1f, 0f);
    }

    public static void DrawCircle(this SpriteBatch sb, Vector2 pos, Vector2 size, Color color, float layer = 1f)
    {
        const int points = 30;
        const float step = (float)((Math.PI * 2) / points);
        float totalStep = 0f;
        Vector2 p1 = new Vector2(
            pos.X + size.X * (float)Math.Cos(totalStep),
            pos.Y + size.Y * (float)Math.Sin(totalStep));
        Vector2 p2;
        for (int i = 0; i <= points; i++)
        {
            p2.X = pos.X + size.X * (float)Math.Cos(totalStep);
            p2.Y = pos.Y + size.Y * (float)Math.Sin(totalStep);
            totalStep += step;
            DrawHelper.DrawLine(p1, p2, color, 1, layer);
            p1 = p2;
        }
    }

    public static void DrawCircleFilled(this SpriteBatch sb, Vector2 pos, float rad)
    {
        DrawCircleFilled(sb, pos, rad, Color.White);
    }

    public static void DrawCircleFilled(this SpriteBatch sb, Vector2 pos, float rad, Color color)
    {
        DrawCircleFilled(sb, pos, rad, color, 1f);
    }

    public static void DrawCircleFilled(this SpriteBatch sb, Vector2 pos, float rad, Color color, float layer)
    {
        // todo
    }

}
