using System.Text.Json;
using Microsoft.Xna.Framework.Content;

namespace Fort.MG.PipelineExtensionReaders;

public class JsonTypeReader<T> : ContentTypeReader<T>
{
	protected override T Read(ContentReader input, T existingInstance)
	{
		var json = input.ReadString();

		// Dynamically fetch the type
		var contentType = typeof(T);

		var result = (T)JsonSerializer.Deserialize(json, contentType);
		return result;
	}
}
