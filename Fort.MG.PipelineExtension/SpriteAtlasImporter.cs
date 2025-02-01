using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Collections.Generic;
using System.IO;
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
		var data = JsonSerializer.Deserialize<Atlas>(json);
		var atlas = data;

		if (!Directory.Exists(atlas.sourceFolder))
			throw new DirectoryNotFoundException(atlas.sourceFolder);

		var files = Directory.GetFiles(atlas.sourceFolder, "*.png");

		var texImporter = new TextureImporter();
		var textures = new List<TextureContent>(files.Length);
		foreach (var f in files)
		{
			context.AddDependency(f);
			var texture = texImporter.Import(f, context);
			texture.Name = Path.GetFileNameWithoutExtension(f);
			textures.Add(texture);
		}

		atlas.textures = textures;

		return data;
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
