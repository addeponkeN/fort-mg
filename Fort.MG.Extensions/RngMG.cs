using Fort.Utility;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public class RngMG
{
	public static Vector2 RngDirection => Vector2.Normalize(new Vector2(Rng.NextFloat(-1f, 1f), Rng.NextFloat(-1, 1f)));

	public static Color RandomColor()
	{
		return new Color(Rng.Next(255), Rng.Next(255), Rng.Next(255));
	}

	public static Vector2 PositionInRadius(float radius, Vector2 position)
	{
		return new Vector2(
			position.X + MathF.Sin(Rng.NextFloat(MathF.PI * 2f)) * radius,
			position.Y + MathF.Cos(Rng.NextFloat(MathF.PI * 2f)) * radius);
	}

	public static Vector2 PositionOnCircle(float radius, Vector2 position)
	{
		return new Vector2(
			position.X + MathF.Sin(Rng.NextFloat(MathF.PI * 2)) * radius,
			position.Y + MathF.Cos(Rng.NextFloat(MathF.PI * 2)) * radius);
	}

}
