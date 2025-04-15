using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class Window : StackPanel
{
	public Window()
	{
		base.Size = new Vector2(300, 180);
		AutoSize = false;
	}

	public override void Start()
	{
		base.Start();
		if (Skins.Count <= 0)
			AddSkin(Skin.DefaultSkinBackground1);
	}


	public override void Draw()
	{
		var sb = Graphics.SpriteBatch;

		var tf = Canvas.TransformMatrix;

		sb.Begin(samplerState: Canvas.SamplerState, transformMatrix: tf);
		DrawSelf();
		DrawSkins();
		DrawComponents();
		sb.End();

		base.DrawContent();

		sb.Begin(samplerState: Canvas.SamplerState, transformMatrix: tf);
		DrawItems();
		if (GuiSettings.Debug)
			base.DrawDebug();
		sb.End();

		sb.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.AnisotropicClamp, transformMatrix: tf);
		base.DrawText();
		sb.End();
	}

	public override void DrawContent()
	{
		// dont draw base
	}
	public override void DrawText()
	{
		// dont draw base
	}
}
