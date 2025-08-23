using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Fort.MG.PipelineExtension;

/// <summary>
/// Transforms a directory of PNG images into a single sprite atlas.
/// </summary>

[ContentProcessor(DisplayName = "Sprite Atlas Processor - Fort")]
public class SpriteAtlasProcessor : ContentProcessor<Atlas, SpriteAtlasContent>
{
	[DisplayName("Region padding")] public int Padding { get; set; } = 0;
	[DisplayName("Add pixel")] public bool IsAddPixel { get; set; } = true;

	public override SpriteAtlasContent Process(Atlas input, ContentProcessorContext context)
	{
		return Pack(input);
	}

	private SpriteAtlasContent Pack(Atlas atlas)
	{
		Console.WriteLine("Packing started");

		if (IsAddPixel && atlas.textures.All(x => x.Name != "pixel"))
		{
			var pixelBitmap = new PixelBitmapContent<Color>(1, 1);
			pixelBitmap.SetPixel(0, 0, Color.White);

			var pixelTexture = new Texture2DContent { Name = "pixel" };
			pixelTexture.Faces[0].Add(pixelBitmap);

			atlas.textures.Add(pixelTexture);
		}

		var images = new BitmapContent[atlas.textures.Count];
		var rectangles = new RectpackSharp.PackingRectangle[atlas.textures.Count];
		var regions = new SpriteRegionContent[atlas.textures.Count];

		Console.WriteLine("Setting rectangles");
		for (int i = 0; i < atlas.textures.Count; i++)
		{
			var texture = atlas.textures[i];

			if (texture.Faces.Count == 0 || texture.Faces[0].Count == 0)
				throw new InvalidContentException(
					$"Texture {texture.Name} has no faces/bitmaps (import failed)");

			var face = texture.Faces.First();
			var bitmap = face.First();
			if (bitmap == null)
				throw new InvalidContentException($"Texture {texture.Name} has a null bitmap");

			images[i] = bitmap;

			rectangles[i] = new RectpackSharp.PackingRectangle
			{
				Id = i,
				Width = (uint)(bitmap.Width + Padding * 2),
				Height = (uint)(bitmap.Height + Padding * 2)
			};
		}

		Console.WriteLine("Packing 'RectanglePacker.Pack()'");
		RectpackSharp.RectanglePacker.Pack(rectangles, out RectpackSharp.PackingRectangle bounds);

		var width = (int)bounds.Width;
		var height = (int)bounds.Height;

		var resultImage = new PixelBitmapContent<Color>(width, height);

		int idx = 0;
		foreach (var rect in rectangles)
		{
			var image = images[rect.Id];
			var srcRect = new Rectangle(0, 0, image.Width, image.Height);
			var destRect = new Rectangle((int)rect.X + Padding, (int)rect.Y + Padding, image.Width, image.Height);

			BitmapContent.Copy(image, srcRect, resultImage, destRect);

			var name = atlas.textures[rect.Id].Name;
			var region = new SpriteRegionContent(name, destRect);
			regions[idx++] = region;
		}

		var retTexture = new Texture2DContent();
		retTexture.Mipmaps.Add(resultImage);

		Console.WriteLine($"Atlas processed: {atlas.name}, {regions.Length} regions");
		return new SpriteAtlasContent(atlas.name, retTexture, regions);
	}
}

public class SpriteAtlasContent
{
	public string Name { get; set; }
	public Texture2DContent Texture { get; }
	public SpriteRegionContent[] Regions { get; }

	public SpriteAtlasContent(string name, Texture2DContent texture, SpriteRegionContent[] regions)
	{
		Texture = texture;
		Regions = regions;
		Name = name;
	}
}

public class SpriteRegionContent
{
	public string Name { get; set; }
	public Rectangle Frame { get; set; }
	public SpriteRegionContent(string name, Rectangle frame)
	{
		Name = name;
		Frame = frame;
	}
}