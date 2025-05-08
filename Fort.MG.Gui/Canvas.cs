using Fort.MG.Gui.Components;
using Fort.MG.VirtualViewports;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

public class Canvas : Container
{
	public static Matrix DefaultScaleTransform => Matrix.CreateScale(Screen.Width / 1280f, Screen.Height / 720f, 1f);
	
	private readonly FocusManager _focusManager = new();
	private RenderTarget2D _target;

	public List<Window> Windows { get; private set; } = new();
	protected List<GuiComponent> _items = new();

	internal SpriteBatch Sb;

	public VirtualViewport VirtualViewport { get; set; }
	public Matrix TransformMatrix => VirtualViewport.Matrix;

	public BlendState BlendState { get; set; } = BlendState.AlphaBlend;
	public SamplerState SamplerState { get; set; } = SamplerState.PointClamp;

	public Vector2 MousePosition => Input.MouseTransformedPos(TransformMatrix);
	public GuiComponent FocusedComponent => _focusManager.FocusedComponent;

	public bool FitScreen { get; set; } = true;

	public Canvas(int virtualWidth = 0, int virtualHeight = 0)
	{
		GuiContent.Load();
		Sb = Graphics.SpriteBatch;
		AutoSize = false;
		VirtualViewport = new();
		UpdateCanvasSize();

		virtualWidth = virtualWidth > 0 ? virtualWidth : Screen.Width;
		virtualHeight = virtualHeight > 0 ? virtualHeight : Screen.Height;
		SetVirtualSize(virtualWidth, virtualHeight);

		Screen.OnScreenSizeChanged += UpdateCanvasSize;
	}

	public T? GetWindow<T>() where T : Window
	{
		return Windows.Find(w => w is T) as T;
	}

	private void UpdateCanvasSize()
	{
		if (FitScreen)
		{
			Size = new Vector2(Screen.Width, Screen.Height);
			_target?.Dispose();
			_target = new RenderTarget2D(Graphics.GraphicsDevice, Screen.Width, Screen.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
		}
	}

	public void SetVirtualSize(int w, int h)
	{
		VirtualViewport = new VirtualViewportScaling(w, h);
	}

	public override void AddItem(GuiComponent item)
	{
		item.Canvas = this;
		item.Parent = this;
		Items.Add(item);

		if (item is Window w)
		{
			Windows.Insert(0, w);
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

	public override void Start()
	{
		_focusManager.Update(this);
		base.Start();
	}

	public override void RemoveItem(GuiComponent item)
	{
		item._canvas = null;
		Items.Remove(item);
		if (item is Window w)
		{
			Windows.Remove(w);
		}
		else
		{
			_items.Remove(item);
		}
	}

	public override void Update(GameTime gt)
	{
		var inputHandlerArgs = new InputHandlerArgs(MousePosition);

		UpdateInput(inputHandlerArgs);
		base.Update(gt);
	}

	public override void UpdateInput(InputHandlerArgs args)
	{
		base.UpdateInput(args);

		_focusManager.UpdateInput(args);
		if (Input.LeftClick)
		{
			_focusManager.HandleMouseClick(MousePosition);
		}
	}

	public void Render()
	{
		var gd = Sb.GraphicsDevice;

		foreach (var win in Windows)
		{
			if (!win.IsVisible) continue;
			win.DrawTarget();
		}

		gd.SetRenderTarget(_target);
		gd.Clear(Color.Transparent);

		foreach (var win in Windows)
		{
			if (!win.IsVisible) continue;
			win.Draw();
		}

		Sb.Begin(SpriteSortMode.Deferred,
			BlendState.AlphaBlend,
			SamplerState.PointClamp,
			null, null, null,
			TransformMatrix);
		foreach (var item in _items)
		{
			if (!item.IsVisible)
				continue;
			item.Draw();
		}

		_focusManager.DrawSelection();
		Sb.End();
	}

	public override void Draw()
	{
		Sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
		Sb.Draw(_target, Vector2.Zero, Color.White);
		Sb.End();

	}

	public override void DrawDebug()
	{
		base.DrawDebug();
	}

}
