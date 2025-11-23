namespace Fort.MG.TilemapEngine;

public static class TileExtensions
{
	/// <summary>
	/// Checks if a tile is empty (air)
	/// </summary>
	public static bool IsEmpty(this Tile? tile)
	{
		return tile == null || tile.Type == 0;
	}

	/// <summary>
	/// Checks if two tiles are the same type
	/// </summary>
	public static bool IsSameType(this Tile? tile, Tile? other)
	{
		if (tile == null || other == null) return false;
		return tile.Type == other.Type;
	}

	/// <summary>
	/// Check if the tile can autotile to the other tile
	/// </summary>
	public static bool CanConnectTo(this Tile t, Tile? other)
	{
		if (other == null) return false;

		return t.Type == other.Type && t.Type != 0; // 0 = empty/air (?)
	}
}