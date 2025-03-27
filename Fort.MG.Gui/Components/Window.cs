using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui.Components;

public class Window : StackPanel
{

	public Window()
	{
	}

	public override void Draw()
	{
		var sb = Graphics.SpriteBatch;

		sb.Begin(samplerState: Canvas.SamplerState);
		DrawSelf();
		DrawSkins();
		DrawComponents();
		sb.End();

		base.DrawContent();

		sb.Begin(samplerState: Canvas.SamplerState);
		DrawItems();
		//base.DrawDebug();
		sb.End();


		sb.Begin(samplerState: SamplerState.AnisotropicClamp);
		base.DrawText();
		sb.End();
	}

	public override void DrawText()
	{
		base.DrawText();
	}
}
