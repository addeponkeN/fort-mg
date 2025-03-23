using FontStashSharp;
using Fort.MG.Gui.Components;
using Fort.MG.VirtualViewports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

public class Canvas : Container
{
	private List<Window> _windows = new();
	private List<GuiComponent> _items = new();
	private RenderTarget2D _target;
	private TextRenderer _textRen;

	internal SpriteBatch Sb;


	public VirtualViewport VirtualViewport { get; set; }
	public Matrix TransformMatrix => VirtualViewport.Matrix;

	public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
	public SamplerState SamplerState { get; set; } = SamplerState.PointClamp;

	public Vector2 MousePosition => Input.MouseTransformedPos(TransformMatrix);

	public Canvas()
	{
		GuiContent.Load();
		Sb = Graphics.SpriteBatch;
		_textRen = new TextRenderer();
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

	public override void Add(GuiComponent item)
	{
		item.Canvas = this;
		item.Parent = this;
		Items.Add(item);

		if (item is Window w)
		{
			_windows.Insert(0, w);
		}
		else
		{
			_items.Add(item);
		}

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
		if (item is Window w)
		{
			_windows.Remove(w);
		}
		else
		{
			_items.Remove(item);
		}
	}

	public override void Update(GameTime gt)
	{
		UpdateInput();
		base.Update(gt);
	}

	public void Render()
	{
		var gd = Sb.GraphicsDevice;
		gd.SetRenderTarget(_target);
		gd.Clear(Color.Transparent);

		foreach (var w in _windows)
		{
			w.Draw();
		}

		Sb.Begin(SpriteSortMode.Deferred, blendState: BlendState, samplerState: SamplerState);
		foreach (var item in _items)
		{
			item.Draw();
		}
		Sb.End();
	}

	public override void Draw()
	{
		Sb.Begin(samplerState: SamplerState.PointClamp);
		var scale = Size / new Vector2(VirtualViewport.Width, VirtualViewport.Height);
		var rec = new Rectangle(0, 0, (int)VirtualViewport.Width, (int)VirtualViewport.Height);
		Sb.Draw(_target, Position, rec, Style.Foreground, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		//DrawDebug();
		Sb.End();

	}

	public override void DrawDebug()
	{
		base.DrawDebug();
	}

}
