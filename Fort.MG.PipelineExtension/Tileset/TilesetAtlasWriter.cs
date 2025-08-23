using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Fort.MG.PipelineExtension;

[ContentTypeWriter]
public class TilesetAtlasWriter : ContentTypeWriter<TilesetAtlasContent>
{
	public override string GetRuntimeReader(TargetPlatform targetPlatform)
	{
		return "Fort.MG.PipelineExtensionReaders.TilesetAtlasReader, Fort.MG.PipelineExtensionReaders";
	}

	protected override void Write(ContentWriter output, TilesetAtlasContent value)
	{
		Console.WriteLine($"Writing tileset containing {value.Tiles.Count} tiles");

		// Write the texture
		output.WriteObject(value.Texture);

		// Write the regions
		output.Write(value.Tiles.Count);
		foreach (var t in value.Tiles)
		{
			var frame = t.Frame;
			output.Write(t.Tileset);
			output.Write((byte)t.Orientation);
			output.Write((byte)t.Variant);
			output.Write((bool)t.IsSolo);
			output.Write((ushort)frame.X);
			output.Write((ushort)frame.Y);
			output.Write((byte)frame.Width);
			output.Write((byte)frame.Height);

			Console.WriteLine($"Write tile: {t.Tileset},{t.Orientation},{t.Variant} ({t.Frame})");
		}
	}
}
