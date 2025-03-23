using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class GHelper
{
	public static Vector2 Center(Vector2 position, Vector2 sourceSize, Vector2 targetSize)
	{
		return new Vector2(position.X + targetSize.X * 0.5f - sourceSize.X * 0.5f, position.Y + targetSize.Y * 0.5f - sourceSize.Y * 0.5f);
	}

	public static Vector2 Center(Rectangle target, Vector2 sourceSize)
	{
		return new Vector2(target.X + target.Width * 0.5f - sourceSize.X * 0.5f, target.Y + target.Height * 0.5f - sourceSize.Y * 0.5f);
	}
}
