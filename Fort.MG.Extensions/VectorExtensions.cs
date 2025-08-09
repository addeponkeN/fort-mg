using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class VectorExtensions
{
	public static Vector3 ToVector3(this Vector2 v) => new Vector3(v.X, v.Y, 0f);
}
