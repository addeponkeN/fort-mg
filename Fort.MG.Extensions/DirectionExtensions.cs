using Fort.Utility;
using Microsoft.Xna.Framework;

namespace Fort.MG.Extensions;

public static class DirectionExtensions
{
	public static Point ToPoint(this Direction dir)
	{
		switch (dir)
		{
			case Direction.NONE: return new Point(0);
			case Direction.UP: return new Point(0, -1);
			case Direction.DOWN: return new Point(0, 1);
			case Direction.LEFT: return new Point(-1, 0);
			case Direction.RIGHT: return new Point(1, 0);
			case Direction.UP_LEFT: return new Point(-1, -1);
			case Direction.UP_RIGHT: return new Point(1, -1);
			case Direction.DOWN_LEFT: return new Point(-1, 1);
			case Direction.DOWN_RIGHT: return new Point(1, 1);
		}
		return Point.Zero;
	}

	public static Vector2 ToVector2(this Direction dir)
	{
		switch (dir)
		{
			case Direction.UP: return new Vector2(0, -1);
			case Direction.DOWN: return new Vector2(0, 1);
			case Direction.LEFT: return new Vector2(-1, 0);
			case Direction.RIGHT: return new Vector2(1, 0);
			case Direction.UP_LEFT: return new Vector2(-1, -1);
			case Direction.UP_RIGHT: return new Vector2(1, -1);
			case Direction.DOWN_LEFT: return new Vector2(-1, 1);
			case Direction.DOWN_RIGHT: return new Vector2(1, 1);
			default: return Vector2.Zero;
		}
	}


	public static Point DirectionToPoint(this Direction d)
	{
		switch (d)
		{
			case Direction.NONE: return new Point(0);
			case Direction.UP: return new Point(0, -1);
			case Direction.DOWN: return new Point(0, 1);
			case Direction.LEFT: return new Point(-1, 0);
			case Direction.RIGHT: return new Point(1, 0);
			case Direction.UP_LEFT: return new Point(-1, -1);
			case Direction.UP_RIGHT: return new Point(1, -1);
			case Direction.DOWN_LEFT: return new Point(-1, 1);
			case Direction.DOWN_RIGHT: return new Point(1, 1);
		}
		return Point.Zero;
	}

	public static Direction PointToDirection(this Point d)
	{
		if (d == Point.Zero)
			return Direction.NONE;
		else if (d.Y == 0)
		{
			if (d.X > 0)
				return Direction.RIGHT;
			else
				return Direction.LEFT;
		}
		else if (d.Y > 0)
		{
			if (d.X == 0)
				return Direction.DOWN;
			else if (d.X > 0)
				return Direction.DOWN_RIGHT;
			else
				return Direction.DOWN_LEFT;
		}
		else
		{
			if (d.X == 0)
				return Direction.UP;
			else if (d.X > 0)
				return Direction.UP_RIGHT;
			else
				return Direction.UP_LEFT;
		}
	}
}
