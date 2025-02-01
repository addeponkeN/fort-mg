using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace Fort.MG.PipelineExtension;

/// <summary>
/// STEP 2: Processor
/// Transforms a directory of PNG images into a single sprite atlas.
/// </summary>

[ContentProcessor(DisplayName = "Sprite Atlas Processor - Fort")]
public class SpriteAtlasProcessor : ContentProcessor<Atlas, SpriteAtlasContent>
{
	[DisplayName("Region padding")] public int Padding { get; set; } = 0;

	public override SpriteAtlasContent Process(Atlas input, ContentProcessorContext context)
	{
		return Pack(input);
	}

	private SpriteAtlasContent Pack(Atlas atlas)
	{
		var images = new BitmapContent[atlas.textures.Count];
		var rectangles = new RectpackSharp.PackingRectangle[atlas.textures.Count];
		var regions = new SpriteRegionContent[atlas.textures.Count];

		for (int i = 0; i < atlas.textures.Count; i++)
		{
			var texture = atlas.textures[i];
			var face = texture.Faces.First();
			var bitmap = face.First();
			images[i] = bitmap;
			rectangles[i] = new RectpackSharp.PackingRectangle
			{
				Id = i,
				Width = (uint)(bitmap.Width + Padding * 2),
				Height = (uint)(bitmap.Height + Padding * 2)
			};
		}

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