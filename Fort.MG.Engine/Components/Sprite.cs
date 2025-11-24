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

    public Rectangle Frame;

    public Texture2D Texture => Region;

    public float DrawLayer { get; set; } = DefaultDrawLayer;
    public RenderPass RenderPass { get; set; } = DefaultRenderPass;

    public Sprite()
    {
        Region = FortEngine.Assets.Pixel;
        Frame = Region;
        Color = Color.White;
    }

    internal override void OnAfterDeserialize()
    {
        base.OnAfterDeserialize();
        Frame = Region;
        if (Transform.Size == Vector2.Zero)
        {
            Transform.Size = Frame.Size.ToVector2();
            Origin = Frame.Size.ToVector2() * .5f;
        }
    }

    public void SetSprite(SpriteRegion region)
    {
        Region = region;
        Frame = region;
        Transform.Size = Frame.Size.ToVector2();
        Origin = Frame.Size.ToVector2() * .5f;
    }

    public void Render()
    {
    }

    public void Draw()
    {
        var pos = Transform.Position + Origin;
        var drawScale = new Vector2(Transform.Size.X / Frame.Width, Transform.Size.Y / Frame.Height);
        Graphics.SpriteBatch.Draw(Texture, pos, Frame, Color, Rotation, Origin, drawScale, SpriteEffect, DrawLayer);
    }

    public override string ToString()
    {
        return $"{Entity}-sprite-{Region.Name}";
    }
}