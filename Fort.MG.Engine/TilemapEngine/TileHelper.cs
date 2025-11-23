using Microsoft.Xna.Framework;

namespace Fort.MG.TilemapEngine;

public static class TileHelper
{
	public static int TileToTileIndex(int x, int y, int chunkX, int chunkY, int chunkSize)
	{
		// Calculate local position within the chunk
		int localX = x - (chunkX * chunkSize);
		int localY = y - (chunkY * chunkSize);

		// Index within the chunk
		return localY * chunkSize + localX;
	}

	public static Point TileIndexToWorld(int tileIndex, int chunkX, int chunkY, int tileSize, int chunkSize)
	{
		// Find the tile's position within the chunk
		int localY = (tileIndex / chunkSize);
		int localX = (tileIndex % chunkSize);
		var chunkSizeWorld = chunkSize * tileSize;

		// Calculate the world coordinates by adding the chunk's offset
		int worldX = chunkX * chunkSizeWorld + localX * tileSize;
		int worldY = chunkY * chunkSizeWorld + localY * tileSize;
		return new Point(worldX, worldY);
	}

	public static Point TileToChunk(int x, int y, int chunkSize)
	{
		// Using integer division for correct chunk coordinates
		int chunkX = x / chunkSize;
		int chunkY = y / chunkSize;
		// Adjust chunk coordinates for negative coordinates by checking the remainder
		if (x < 0 && x % chunkSize != 0) chunkX--;
		if (y < 0 && y % chunkSize != 0) chunkY--;
		return new Point(chunkX, chunkY);
	}

	/// <summary>
	/// Converts chunk coordinates to the top-left tile position of that chunk
	/// </summary>
	/// <param name="chunkX">Chunk X coordinate</param>
	/// <param name="chunkY">Chunk Y coordinate</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <returns>Top-left tile position of the chunk</returns>
	public static Point ChunkToTile(int chunkX, int chunkY, int chunkSize)
	{
		return new Point(chunkX * chunkSize, chunkY * chunkSize);
	}

	/// <summary>
	/// Converts chunk coordinates to world pixel coordinates (top-left corner)
	/// </summary>
	/// <param name="chunkX">Chunk X coordinate</param>
	/// <param name="chunkY">Chunk Y coordinate</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>World pixel coordinates of chunk's top-left corner</returns>
	public static Point ChunkToWorld(int chunkX, int chunkY, int chunkSize, int tileSize)
	{
		int chunkSizeWorld = chunkSize * tileSize;
		return new Point(chunkX * chunkSizeWorld, chunkY * chunkSizeWorld);
	}

	/// <summary>
	/// Converts world pixel coordinates to tile coordinates
	/// </summary>
	/// <param name="worldX">World X coordinate in pixels</param>
	/// <param name="worldY">World Y coordinate in pixels</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>Tile coordinates</returns>
	public static Point WorldToTile(int worldX, int worldY, int tileSize)
	{
		int tileX = worldX / tileSize;
		int tileY = worldY / tileSize;

		// Adjust for negative coordinates
		if (worldX < 0 && worldX % tileSize != 0) tileX--;
		if (worldY < 0 && worldY % tileSize != 0) tileY--;

		return new Point(tileX, tileY);
	}

	/// <summary>
	/// Converts tile coordinates to world pixel coordinates (top-left corner of tile)
	/// </summary>
	/// <param name="tileX">Tile X coordinate</param>
	/// <param name="tileY">Tile Y coordinate</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>World pixel coordinates of tile's top-left corner</returns>
	public static Point TileToWorld(int tileX, int tileY, int tileSize)
	{
		return new Point(tileX * tileSize, tileY * tileSize);
	}

	/// <summary>
	/// Converts world pixel coordinates to chunk coordinates
	/// </summary>
	/// <param name="worldX">World X coordinate in pixels</param>
	/// <param name="worldY">World Y coordinate in pixels</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>Chunk coordinates</returns>
	public static Point WorldToChunk(int worldX, int worldY, int chunkSize, int tileSize)
	{
		var tilePos = WorldToTile(worldX, worldY, tileSize);
		return TileToChunk(tilePos.X, tilePos.Y, chunkSize);
	}

	/// <summary>
	/// Gets the local tile position within a chunk
	/// </summary>
	/// <param name="tileX">Global tile X coordinate</param>
	/// <param name="tileY">Global tile Y coordinate</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <returns>Local tile position within the chunk (0 to chunkSize-1)</returns>
	public static Point TileToLocal(int tileX, int tileY, int chunkSize)
	{
		int localX = tileX % chunkSize;
		int localY = tileY % chunkSize;

		// Handle negative coordinates
		if (localX < 0) localX += chunkSize;
		if (localY < 0) localY += chunkSize;

		return new Point(localX, localY);
	}

	/// <summary>
	/// Converts local tile position and chunk coordinates to global tile position
	/// </summary>
	/// <param name="localX">Local X position within chunk (0 to chunkSize-1)</param>
	/// <param name="localY">Local Y position within chunk (0 to chunkSize-1)</param>
	/// <param name="chunkX">Chunk X coordinate</param>
	/// <param name="chunkY">Chunk Y coordinate</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <returns>Global tile coordinates</returns>
	public static Point LocalToTile(int localX, int localY, int chunkX, int chunkY, int chunkSize)
	{
		return new Point(chunkX * chunkSize + localX, chunkY * chunkSize + localY);
	}

	/// <summary>
	/// Gets the center world position of a tile
	/// </summary>
	/// <param name="tileX">Tile X coordinate</param>
	/// <param name="tileY">Tile Y coordinate</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>Center world position of the tile</returns>
	public static Point TileToWorldCenter(int tileX, int tileY, int tileSize)
	{
		int halfTile = tileSize / 2;
		return new Point(tileX * tileSize + halfTile, tileY * tileSize + halfTile);
	}

	/// <summary>
	/// Gets the center world position of a chunk
	/// </summary>
	/// <param name="chunkX">Chunk X coordinate</param>
	/// <param name="chunkY">Chunk Y coordinate</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>Center world position of the chunk</returns>
	public static Point ChunkToWorldCenter(int chunkX, int chunkY, int chunkSize, int tileSize)
	{
		int chunkSizeWorld = chunkSize * tileSize;
		int halfChunk = chunkSizeWorld / 2;
		return new Point(chunkX * chunkSizeWorld + halfChunk, chunkY * chunkSizeWorld + halfChunk);
	}

	/// <summary>
	/// Gets all chunk coordinates that intersect with a world rectangle
	/// </summary>
	/// <param name="worldX">World rectangle X</param>
	/// <param name="worldY">World rectangle Y</param>
	/// <param name="width">World rectangle width</param>
	/// <param name="height">World rectangle height</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>Array of chunk coordinates that intersect the rectangle</returns>
	public static Point[] WorldRectToChunks(int worldX, int worldY, int width, int height, int chunkSize, int tileSize)
	{
		var topLeft = WorldToChunk(worldX, worldY, chunkSize, tileSize);
		var bottomRight = WorldToChunk(worldX + width - 1, worldY + height - 1, chunkSize, tileSize);

		var chunks = new List<Point>();

		for (int chunkY = topLeft.Y; chunkY <= bottomRight.Y; chunkY++)
		{
			for (int chunkX = topLeft.X; chunkX <= bottomRight.X; chunkX++)
			{
				chunks.Add(new Point(chunkX, chunkY));
			}
		}

		return chunks.ToArray();
	}

	/// <summary>
	/// Gets all tile coordinates within a world rectangle
	/// </summary>
	/// <param name="worldX">World rectangle X</param>
	/// <param name="worldY">World rectangle Y</param>
	/// <param name="width">World rectangle width</param>
	/// <param name="height">World rectangle height</param>
	/// <param name="tileSize">Size of each tile in pixels</param>
	/// <returns>Array of tile coordinates within the rectangle</returns>
	public static Point[] WorldRectToTiles(int worldX, int worldY, int width, int height, int tileSize)
	{
		var topLeft = WorldToTile(worldX, worldY, tileSize);
		var bottomRight = WorldToTile(worldX + width - 1, worldY + height - 1, tileSize);

		var tiles = new List<Point>();

		for (int tileY = topLeft.Y; tileY <= bottomRight.Y; tileY++)
		{
			for (int tileX = topLeft.X; tileX <= bottomRight.X; tileX++)
			{
				tiles.Add(new Point(tileX, tileY));
			}
		}

		return tiles.ToArray();
	}

	/// <summary>
	/// Calculates the distance between two tiles
	/// </summary>
	/// <param name="tile1X">First tile X coordinate</param>
	/// <param name="tile1Y">First tile Y coordinate</param>
	/// <param name="tile2X">Second tile X coordinate</param>
	/// <param name="tile2Y">Second tile Y coordinate</param>
	/// <returns>Distance in tiles</returns>
	public static float TileDistance(int tile1X, int tile1Y, int tile2X, int tile2Y)
	{
		int deltaX = tile2X - tile1X;
		int deltaY = tile2Y - tile1Y;
		return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
	}

	/// <summary>
	/// Calculates the Manhattan distance between two tiles
	/// </summary>
	/// <param name="tile1X">First tile X coordinate</param>
	/// <param name="tile1Y">First tile Y coordinate</param>
	/// <param name="tile2X">Second tile X coordinate</param>
	/// <param name="tile2Y">Second tile Y coordinate</param>
	/// <returns>Manhattan distance in tiles</returns>
	public static int TileManhattanDistance(int tile1X, int tile1Y, int tile2X, int tile2Y)
	{
		return Math.Abs(tile2X - tile1X) + Math.Abs(tile2Y - tile1Y);
	}

	/// <summary>
	/// Checks if a tile coordinate is within a chunk's bounds
	/// </summary>
	/// <param name="tileX">Tile X coordinate</param>
	/// <param name="tileY">Tile Y coordinate</param>
	/// <param name="chunkX">Chunk X coordinate</param>
	/// <param name="chunkY">Chunk Y coordinate</param>
	/// <param name="chunkSize">Size of chunk in tiles</param>
	/// <returns>True if tile is within the chunk</returns>
	public static bool IsTileInChunk(int tileX, int tileY, int chunkX, int chunkY, int chunkSize)
	{
		var chunkBounds = TileToChunk(tileX, tileY, chunkSize);
		return chunkBounds.X == chunkX && chunkBounds.Y == chunkY;
	}
}
