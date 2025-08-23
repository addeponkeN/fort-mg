using Fort.MG.Assets.Data;
using Fort.MG.EntitySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YamlDotNet.Serialization;

namespace Fort.MG.Components;

public class Sprite : Component
{
	[YamlMember(Alias = "sprite")]
	[Serialize] public SpriteRegion Region;
	[Serialize] public Color Color;
	[Serialize] public float Rotation;
	[Serialize] public Vector2 Origin;
	[Serialize] public SpriteEffects SpriteEffect;

	public float DrawLayer;

	public Texture2D Texture => Region;
	public Rectangle Frame => Region;

	public override void Init()
	{
		base.Init();
		Region = FortEngine.Assets.Pixel;
		Color = Color.White;
	}

	public void SetSprite(SpriteRegion region)
	{
		Region = region;
		Transform.Size = Frame.Size.ToVector2();
	}

	public override void Draw()
	{
		base.Draw();
		var drawScale = new Vector2(Transform.Size.X / Frame.Width, Transform.Size.Y / Frame.Height);
		Graphics.SpriteBatch.Draw(Texture, Transform.Position, Frame, Color, Rotation, Origin, drawScale, SpriteEffect, DrawLayer);
	}
}