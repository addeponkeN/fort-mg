using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Fort.MG.PipelineExtension;

[ContentImporter(".json", DisplayName = "Json Importer - Fort", DefaultProcessor = nameof(JsonProcessor))]
public class JsonImporter : ContentImporter<string>
{
	public override string Import(string filename, ContentImporterContext context)
	{
		var json = File.ReadAllText(filename);
		EnsureJson(json);
		return json;
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
