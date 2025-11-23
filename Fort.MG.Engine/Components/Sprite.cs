using Fort.MG.Assets.Data;
using Fort.MG.EntitySystem;
using Fort.MG.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using YamlDotNet.Serialization;

namespace Fort.MG.Components;

public class Sprite : Component, IFortRenderable
{
    public static float DefaultDrawLayer = 0f;
    public static RenderPass DefaultRenderPass = RenderPasses.Default;

    [YamlMember(Alias = "sprite")]
    [Serialize] public SpriteRegion Region;
    [Serialize] public Color Color;
    [Serialize] public float Rotation;
    [Serialize] public Vector2 Origin;
    [Serialize] public SpriteEffects SpriteEffect;

    public float DrawLayer { get; set; } = DefaultDrawLayer;

    public RenderPass RenderPass { get; set; } = DefaultRenderPass;

    public Texture2D Texture => Region;
    public Rectangle Frame => Region;

    public Sprite()
    {
        Region = FortEngine.Assets.Pixel;
        Color = Color.White;
    }

    internal override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
        if (Transform.Size == Vector2.Zero)
            Transform.Size = Frame.Size.ToVector2();
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