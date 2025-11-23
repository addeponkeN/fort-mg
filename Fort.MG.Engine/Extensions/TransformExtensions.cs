using Fort.MG.EntitySystem;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class TransformExtensions
{
    public static Vector2 Center(this Transform t)
    {
        return t.Position + t.Size * 0.5f;
    }

    public static Vector2 BotMid(this Transform t)
    {
        return t.Position + new Vector2(t.Size.X * 0.5f, t.Size.Y);
    }

    public static Vector2 BotRight(this Transform t)
    {
        return t.Position + t.Size;
    }

    public static Rectangle Bounds(this Transform t)
    {
        var pos = t.Position;
        var size = t.Size;
        return new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
    }
}
