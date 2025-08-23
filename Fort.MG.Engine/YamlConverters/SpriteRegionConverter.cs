using Fort.MG.Assets.Data;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Fort.MG.YamlConverters;

public class SpriteRegionYamlConverter : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(SpriteRegion);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
	{
		var spriteName = parser.Consume<Scalar>().Value ?? "";
		var sprite = FortEngine.Assets.GetSprite(spriteName);
		return new SpriteRegion(spriteName, sprite, sprite);
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
	{
		var region = (SpriteRegion)value!;
		// Only emit the sprite name
		emitter.Emit(new Scalar(region.Name));
	}
}
