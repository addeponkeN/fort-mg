using System;
using System.ComponentModel;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Fort.MG.PipelineExtension;

[ContentProcessor(DisplayName = "Json Processor - Fort")]
public class JsonProcessor : ContentProcessor<string, JsonProcessResult>
{
	[DisplayName("Runtime")]
	public string Runtime { get; set; } = string.Empty;

	public override JsonProcessResult Process(string input, ContentProcessorContext context)
	{
		if (string.IsNullOrEmpty(Runtime))
		{
			throw new Exception("Runtime is not set");
		}

		var result = new JsonProcessResult
		{
			Content = input,
			Runtime = Runtime,
		};

		return result;
	}
}