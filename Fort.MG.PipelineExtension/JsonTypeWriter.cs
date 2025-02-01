using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Fort.MG.PipelineExtension;

[ContentTypeWriter]
public class JsonTypeWriter : ContentTypeWriter<JsonProcessResult>
{
	private string _runtime;

	public override string GetRuntimeReader(TargetPlatform targetPlatform)
	{
		return _runtime;
	}

	protected override void Write(ContentWriter output, JsonProcessResult value)
	{
		_runtime = value.Runtime;
		output.Write(value.Content);
	}
}
