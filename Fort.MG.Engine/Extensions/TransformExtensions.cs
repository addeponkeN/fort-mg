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
}
