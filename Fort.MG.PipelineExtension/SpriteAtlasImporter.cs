using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Text.Json;

namespace Fort.MG.PipelineExtension;

/// <summary>
/// STEP 1: Importer
/// Reads from file and imports into the content pipeline.
/// </summary>

[ContentImporter(".atlas", DisplayName = "Sprite Atlas Importer - Fort", DefaultProcessor = nameof(SpriteAtlasProcessor))]
public class SpriteAtlasImporter : ContentImporter<Atlas>
{
	public override Atlas Import(string filename, ContentImporterContext context)
	{
		var json = File.ReadAllText(filename);

		EnsureJson(json);

		var atlas = JsonSerializer.Deserialize<Atlas>(json);

		if (!Directory.Exists(atlas.sourceFolder))
			throw new DirectoryNotFoundException(atlas.sourceFolder);

		var files = Directory.GetFiles(atlas.sourceFolder, "*.png");

		var texImporter = new TextureImporter();
		var textures = new List<TextureContent>(files.Length);

		foreach (var f in files)
		{
			context.AddDependency(f);
			var texture = texImporter.Import(f, context);

			if (texture == null)
				throw new InvalidContentException($"Failed to import texture {f}");

			if (texture.Faces.Count == 0 || texture.Faces[0].Count == 0)
				throw new InvalidContentException($"Texture {f} has no valid bitmap data");

			texture.Name = Path.GetFileNameWithoutExtension(f);
			textures.Add(texture);
		}

		atlas.name = string.IsNullOrEmpty(atlas.name) ? Path.GetFileNameWithoutExtension(filename) : atlas.name;
		atlas.textures = textures;

		Console.WriteLine($"Found {textures.Count} images in {atlas.sourceFolder}");

		return atlas;
	}

	private void EnsureJson(string json)
	{
		try
		{
			JsonDocument.Parse(json);
		}
		catch (Exception e)
		{
			throw new InvalidContentException("Invalid JSON", e);
		}
	}
}
