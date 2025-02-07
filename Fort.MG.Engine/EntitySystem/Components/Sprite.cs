﻿using Fort.MG.Assets.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.EntitySystem.Components;

public class Sprite : Component
{
	public Texture2D Texture;
    public Rectangle Frame;
    public Color Color;
    public float Rotation;
    public Vector2 Origin;
    public SpriteEffects SpriteEffect;
    public float DrawLayer;

    public override void Init()
    {
        base.Init();
        Texture = FortEngine.AssetManager.Pixel;
        Frame = FortEngine.AssetManager.Pixel;
        Color = Color.White;
    }

    public void SetSprite(SpriteRegion region)
    {
        Texture = region;
        Frame = region;
        Transform.Size = Frame.Size.ToVector2();
    }
    
    public override void Draw()
    {
        base.Draw();
        var drawScale = new Vector2(Transform.Size.X / Frame.Width, Transform.Size.Y / Frame.Height);
        Graphics.SpriteBatch.Draw(Texture, Transform.Position, Frame, Color, Rotation, Origin, drawScale, SpriteEffect, DrawLayer);
    }
}