using Microsoft.Xna.Framework;

namespace Fort.MG.TilemapEngine;

public static class TileMapExtensions
{
	public static Rectangle WorldBoundsToChunk(this TileMap map, Rectangle bounds)
	{
		return WorldBoundsToChunk(map, bounds.X, bounds.Y, bounds.Width, bounds.Height);
	}

	public static Rectangle WorldBoundsToChunk(this TileMap map, int x, int y, int w, int h)
	{
		int chunkSizePx = map.TileSize * map.ChunkSize;

		// Compute the starting chunk coordinates using integer division
		int startChunkX = x >= 0 ? x / chunkSizePx : (x - chunkSizePx + 1) / chunkSizePx;
		int startChunkY = y >= 0 ? y / chunkSizePx : (y - chunkSizePx + 1) / chunkSizePx;

		// Compute the ending chunk coordinates (adjusting for bounds)
		int endChunkX = (x + w - 1) >= 0 ? (x + w - 1) / chunkSizePx : ((x + w - 1) - chunkSizePx + 1) / chunkSizePx;
		int endChunkY = (y + h - 1) >= 0 ? (y + h - 1) / chunkSizePx : ((y + h - 1) - chunkSizePx + 1) / chunkSizePx;

		// Width and height in chunks
		int chunkWidth = endChunkX - startChunkX + 1;
		int chunkHeight = endChunkY - startChunkY + 1;

		return new Rectangle(startChunkX, startChunkY, chunkWidth, chunkHeight);
	}

	public static Rectangle WorldBoundsToTile(this TileMap map, Rectangle bounds)
	{
		return WorldBoundsToChunk(map, bounds.X, bounds.Y, bounds.Width, bounds.Height);
	}
	public static Rectangle WorldBoundsToTile(this TileMap map, int x, int y, int w, int h)
	{
		// Assuming map.TileSize is a power of 2
		int tileSizeShift = (int)Math.Log2(map.TileSize);

		// Compute the starting tile coordinates using bit-shifting
		int startTileX = x >= 0 ? x >> tileSizeShift : (x - map.TileSize + 1) >> tileSizeShift;
		int startTileY = y >= 0 ? y >> tileSizeShift : (y - map.TileSize + 1) >> tileSizeShift;

		// Compute the ending tile coordinates (adjusting for bounds)
		int endTileX = (x + w - 1) >= 0 ? (x + w - 1) >> tileSizeShift : ((x + w - 1) - map.TileSize + 1) >> tileSizeShift;
		int endTileY = (y + h - 1) >= 0 ? (y + h - 1) >> tileSizeShift : ((y + h - 1) - map.TileSize + 1) >> tileSizeShift;

		// Width and height in tiles
		int tileWidth = endTileX - startTileX + 1;
		int tileHeight = endTileY - startTileY + 1;

		return new Rectangle(startTileX, startTileY, tileWidth, tileHeight);
	}


	public static Rectangle WorldBounds(this Chunk c, int cs, int ts)
	{
		int x = c.X * cs * ts;
		int y = c.Y * cs * ts;
		int w = cs * ts;
		int h = cs * ts;
		return new Rectangle(x, y, w, h);
	}

}
