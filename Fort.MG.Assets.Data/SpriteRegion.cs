using System.Text.Json.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets.Data;

public class SpriteRegion
{
	public string Name { get; set; }
	public Rectangle Frame { get; set; }
	[JsonIgnore] public Texture2D Texture { get; set; }
	public SpriteRegion(string name, Texture2D texture, Rectangle frame)
	{
		Name = name;
		Texture = texture;
		Frame = frame;
	}

	public static implicit operator Texture2D(SpriteRegion tf) => tf.Texture;
	public static implicit operator Rectangle(SpriteRegion tf) => tf.Frame;

	public override string ToString()
	{
		return $"{Name}: {Frame}";
	}
}
