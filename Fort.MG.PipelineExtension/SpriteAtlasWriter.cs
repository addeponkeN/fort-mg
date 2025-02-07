using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Fort.MG.PipelineExtension;

/// <summary>
/// Write data to .xnb file
/// </summary>
[ContentTypeWriter]
public class SpriteAtlasWriter : ContentTypeWriter<SpriteAtlasContent>
{
	protected override void Write(ContentWriter output, SpriteAtlasContent value)
	{
		// Write the texture
		output.WriteObject(value.Texture);

		// Write the regions
		output.Write(value.Regions.Length);
		foreach (var region in value.Regions)
		{
			var frame = region.Frame;
			output.Write(region.Name);
			output.Write(frame.X);
			output.Write(frame.Y);
			output.Write(frame.Width);
			output.Write(frame.Height);
		}
	}

	public override string GetRuntimeReader(TargetPlatform targetPlatform)
	{
		return "Fort.MG.PipelineExtensionReaders.SpriteAtlasReader, Fort.MG.PipelineExtensionReaders";
	}
}