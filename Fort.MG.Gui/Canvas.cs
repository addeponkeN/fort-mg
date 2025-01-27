using Fort.MG.Core;
using Fort.MG.Core.VirtualViewports;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

public class Canvas : Container
{
	private RenderTarget2D _target;

	internal SpriteBatch Sb;

	public VirtualViewport VirtualViewport { get; set; }
	public Matrix TransformMatrix => VirtualViewport.Matrix;

	public int Width, Height;

	public Canvas()
	{
		Sb = Graphics.SpriteBatch;

		UpdateDimensions();
		VirtualViewport = new VirtualViewportScaling(854, 480);
		SetCanvasSize(VirtualViewport.Width, VirtualViewport.Height);

		FortCore.WindowSizeChanged += (sender, args) => UpdateDimensions();
	}

	private void UpdateDimensions()
	{
		Width = Screen.Width;
		Height = Screen.Height;
	}

	private void SetCanvasSize(int w, int h)
	{
		_target?.Dispose();
		Size = new Vector2(w, h);
		_target = new(Sb.GraphicsDevice, w, h);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
	}

	public override void Add(Component item)
	{
		item.Canvas = this;
		item.Parent = this;
		Items.Add(item);
	}

	public override void Remove(Component item)
	{
		item.Canvas = null;
		Items.Remove(item);
	}

	public void Render()
	{
		var gd = Sb.GraphicsDevice;
		gd.SetRenderTarget(_target);
		gd.Clear(Color.Transparent);

		Sb.Begin(SpriteSortMode.BackToFront, transformMatrix: TransformMatrix);
		base.Draw();
		Sb.End();
	}

	public override void Draw()
	{
		Sb.Begin();
		var rec = new Rectangle(0, 0, Width, Height);
		Sb.Draw(_target, Position, rec, Style.Foreground, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
		Sb.End();
	}
}
