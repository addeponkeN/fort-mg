using BCnEncoder.Shared.ImageFiles;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using RectpackSharp;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Color = Microsoft.Xna.Framework.Color;
using PixelFormat = BCnEncoder.Encoder.PixelFormat;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Fort.MG.PipelineExtension;

[ContentProcessor]
internal class TilesetAtlasProcessor : ContentProcessor<TilesetCollection, TilesetAtlasContent>
{
	[DisplayName("Texture padding")] public int Padding { get; set; } = 4;
	[DisplayName("Tile size")] public int TileSize { get; set; } = 32;

	public override TilesetAtlasContent Process(TilesetCollection input, ContentProcessorContext context)
	{
		Console.WriteLine("Tileset packing started");

		var tilesets = input.sets;

		int tileCount = 0;
		int tileId = 0;

		var setPacks = new List<TilesetPack>();
		var rects = new List<PackingRectangle>();
		var allTiles = new List<Tile>();

		foreach (var tileset in tilesets)
		{
			var face = tileset.texture.Faces.First();
			var bitmap = face.First();

			if (bitmap == null)
				throw new InvalidContentException($"Tileset {tileset.name} has a null bitmap");

			var tiles = CutTiles(bitmap, tileset, ref tileId);
			tileCount += tiles.Count;

			var set = new TilesetPack(tileset.name, tiles);
			set.Texture = bitmap;
			setPacks.Add(set);
			allTiles.AddRange(tiles);

			for (int i = 0; i < tiles.Count; i++)
			{
				var t = tiles[i];
				rects.Add(new PackingRectangle
				{
					Id = t.Id,
					Width = (uint)(TileSize + Padding * 2),
					Height = (uint)(TileSize + Padding * 2)
				});
			}

			Console.WriteLine($"Split tileset '{tileset.name}' into '{tiles.Count}' tiles");
		}

		Console.WriteLine($"Packing {rects.Count} tile rectangles - 'RectanglePacker.Pack()'");
		var packRects = rects.ToArray();
		RectanglePacker.Pack(packRects, out PackingRectangle bounds);

		var width = (int)bounds.Width;
		var height = (int)bounds.Height;

		var combinedBitmap = new PixelBitmapContent<Color>(width, height);

		var tileRegions = new List<TileRegion>();
		int idx = 0;

		Console.WriteLine("Creating final bitmap");

		for (int i = 0; i < setPacks.Count; i++)
		{
			var set = setPacks[i];

			for (int j = 0; j < set.Tiles.Count; j++)
			{
				var t = set.Tiles[j];
				var rect = packRects[idx];
				var image = set.Texture;

				var srcRect = new Rectangle(t.X, t.Y, TileSize, TileSize);
				var destRect = new Rectangle((int)rect.X + Padding, (int)rect.Y + Padding, TileSize, TileSize);

				BitmapContent.Copy(image, srcRect, combinedBitmap, destRect);

				var tileRegion = new TileRegion();
				tileRegion.Tileset = set.Name;
				tileRegion.Orientation = t.Orientation;
				tileRegion.Variant = t.Variant;
				tileRegion.Frame = new Rectangle(destRect.X, destRect.Y, TileSize, TileSize);
				tileRegion.IsSolo = image.Height == TileSize; // Check if this is a solo tile
				tileRegions.Add(tileRegion);
				Console.WriteLine($"Tile: [{tileRegion.Tileset},{tileRegion.Orientation},{tileRegion.Variant}] [{srcRect}] -> [{destRect}]");

				idx++;
			}
		}

		var resultTexture = new Texture2DContent();
		resultTexture.Mipmaps.Add(combinedBitmap);
		var content = new TilesetAtlasContent
		{
			Texture = resultTexture,
			Tiles = tileRegions
		};

		string debugPath = Path.ChangeExtension(context.OutputFilename, ".debug.png");
		SaveDebugPng(combinedBitmap, debugPath);
		Console.WriteLine("Saved debug atlas: " + debugPath);

		return content;
	}

	private void SaveDebugPng(PixelBitmapContent<Color> bitmap, string path)
	{
		int width = bitmap.Width;
		int height = bitmap.Height;

		using var bmp = new Bitmap(width, height);
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Microsoft.Xna.Framework.Color c = bitmap.GetPixel(x, y);
				var sysColor = System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
				bmp.SetPixel(x, y, sysColor);
			}
		}

		bmp.Save(path, ImageFormat.Png);
	}

	private List<Tile> CutTiles(BitmapContent bitmap, Tileset set, ref int idPool)
	{
		var tiles = new List<Tile>();

		var pixelData = bitmap as PixelBitmapContent<Color>;
		if (pixelData == null)
			throw new InvalidContentException("Expected PixelBitmapContent<Color> in tileset");

		for (int x = 0; x < bitmap.Width; x += TileSize)
		{
			for (int y = 0; y < bitmap.Height; y += TileSize)
			{
				if (!IsValidTile(pixelData, x, y))
				{
					continue;
				}

				bool isSolo = bitmap.Height == TileSize;

				int ori = isSolo ? 15 : y / TileSize;
				int variant = x / TileSize;

				var tile = new Tile
				{
					Id = ++idPool,
					X = x,
					Y = y,
					Tileset = set.name,
					Orientation = ori,
					Variant = variant,
				};
				tiles.Add(tile);
			}
		}

		return tiles;
	}

	private bool IsValidTile(PixelBitmapContent<Color> bitmapPixels, int x, int y)
	{
		if (x < 0 || y < 0 || x >= bitmapPixels.Width || y >= bitmapPixels.Height)
			return false;

		int maxX = Math.Min(x + TileSize, bitmapPixels.Width);
		int maxY = Math.Min(y + TileSize, bitmapPixels.Height);

		for (int py = y; py < maxY; py++)
		{
			for (int px = x; px < maxX; px++)
			{
				var color = bitmapPixels.GetPixel(px, py);

				if (color.A > 0)
					return true;
			}
		}

		return false;
	}

}

public class TilesetAtlasContent
{
	public Texture2DContent Texture { get; set; }
	public List<TileRegion> Tiles { get; set; } = new();
}

public class TileRegion
{
	public Rectangle Frame { get; set; }
	public string Tileset { get; set; }
	public int Orientation { get; set; }
	public int Variant { get; set; }
	public bool IsSolo { get; set; }
}

public class Tile
{
	public int Id { get; set; }
	public int X { get; set; }
	public int Y { get; set; }
	public string Tileset { get; set; }
	public int Orientation { get; set; }
	public int Variant { get; set; }
}

public class TilesetPack
{
	public string Name { get; set; }
	public List<Tile> Tiles { get; }

	public BitmapContent Texture { get; set; }

	public TilesetPack(string name, List<Tile> tiles)
	{
		Tiles = tiles;
		Name = name;
	}
}
