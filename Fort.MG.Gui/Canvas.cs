using Fort.MG.Gui.Components;
using Fort.MG.VirtualViewports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

public class Canvas : Container
{
	private RenderTarget2D _target;
	internal SpriteBatch Sb;

	public VirtualViewport VirtualViewport { get; set; }
	public Matrix TransformMatrix => VirtualViewport.Matrix;

	public Canvas()
	{
		GuiContent.Load();
		Sb = Graphics.SpriteBatch;

		AutoSize = false;
		VirtualViewport = new();

		UpdateCanvasSize();
		SetVirtualSize(Screen.Width, Screen.Height);

		FortCore.WindowSizeChanged += (sender, args) => UpdateCanvasSize();
	}

	private void UpdateCanvasSize()
	{
		Size = new Vector2(Screen.Width, Screen.Height);
	}

	private void SetVirtualSize(int w, int h)
	{
		VirtualViewport.Width = w;
		VirtualViewport.Height = h;
		_target?.Dispose();
		_target = new(Sb.GraphicsDevice, w, h);
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
	}

	public override void Add(GuiComponent item)
	{
		item._canvas = this;
		item.Parent = this;
		Items.Add(item);

		if (item is Container c)
		{
			foreach (var containerItem in c.Items)
			{
				containerItem._canvas = this;
			}
		}
	}

	public override void Remove(GuiComponent item)
	{
		item._canvas = null;
		Items.Remove(item);
	}

	public void Render()
	{
		var gd = Sb.GraphicsDevice;
		gd.SetRenderTarget(_target);
		gd.Clear(Color.Transparent);

		Sb.Begin(SpriteSortMode.BackToFront);
		base.Draw();
		Sb.End();
	}

	public override void Draw()
	{
		Sb.Begin();
		var scale = Size / new Vector2(VirtualViewport.Width, VirtualViewport.Height);
		var rec = new Rectangle(0, 0, (int)VirtualViewport.Width, (int)VirtualViewport.Height);
		Sb.Draw(_target, Position, rec, Style.Foreground, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		Sb.End();
	}
}
