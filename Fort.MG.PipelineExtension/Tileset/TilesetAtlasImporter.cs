using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Text.Json;

namespace Fort.MG.PipelineExtension;

/// <summary>
/// STEP 1: Importer
/// Reads from file and imports into the content pipeline.
/// </summary>

[ContentImporter(".tilesetatlas", DisplayName = "Tileset Atlas Importer - Fort", DefaultProcessor = nameof(TilesetAtlasProcessor))]
public class TilesetAtlasImporter : ContentImporter<TilesetCollection>
{
	public override TilesetCollection Import(string filename, ContentImporterContext context)
	{
		var json = File.ReadAllText(filename);

		EnsureJson(json);

		var collection = JsonSerializer.Deserialize<TilesetCollection>(json);

		Console.WriteLine(collection);

		if (!Directory.Exists(collection.sourceFolder))
			throw new DirectoryNotFoundException(collection.sourceFolder);

		var tilesetImages = Directory.GetFiles(collection.sourceFolder, "*.png");

		foreach (string f in tilesetImages)
		{
			//if (!f.EndsWith("tileset_grass.png"))
			//	continue;

			Console.WriteLine($"importing '{f}'");

			var texImporter = new TextureImporter();

			context.AddDependency(f);
			var texture = texImporter.Import(f, context);

			if (texture == null)
				throw new InvalidContentException($"Failed to import tileset {f}");

			if (texture.Faces.Count == 0 || texture.Faces[0].Count == 0)
				throw new InvalidContentException($"Tileset {f} has no valid bitmap data");

			texture.Name = Path.GetFileNameWithoutExtension(f);

			collection.sets.Add(new Tileset
			{
				name = texture.Name,
				texture = texture,
			});
		}

		Console.WriteLine($"Found {collection.sets} tilesets in {collection.sourceFolder}");

		return collection;
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
