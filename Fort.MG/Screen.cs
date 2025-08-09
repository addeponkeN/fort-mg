using Microsoft.Xna.Framework;

namespace Fort.MG;

public enum WindowModes
{
	Windowed,
	Borderless,
	Fullscreen,
}

public static class Screen
{
	private static bool _applied = false;
	private static DateTime _dateToggleFullscreen = DateTime.MinValue;
	private static DateTime _dateSetWindowMode = DateTime.MinValue;
	private static int _currentWidth;
	private static int _currentHeight;
	private static bool _currentFullscreen;

	private static int _originalWidth;
	private static int _originalHeight;
	private static WindowModes _windowMode;

	private static GraphicsDeviceManager Gdm => Graphics.GDM;
	private static Game Game => FortCore.Game;
	public static WindowModes WindowMode => _windowMode;
	public static Rectangle Bounds => new Rectangle(0, 0, Width, Height);
	public static Vector2 Dimensions => new Vector2(Width, Height);

	public static event Action OnScreenSizeChanged;

	public static int Width
	{
		get => Gdm.PreferredBackBufferWidth;
		set => Gdm.PreferredBackBufferWidth = value;
	}

	public static int Height
	{
		get => Gdm.PreferredBackBufferHeight;
		set => Gdm.PreferredBackBufferHeight = value;
	}

	public static bool IsFullscreen
	{
		get => Gdm.IsFullScreen;
		set => Gdm.IsFullScreen = value;
	}

	public static bool VSync
	{
		get => Gdm.SynchronizeWithVerticalRetrace;
		set => Gdm.SynchronizeWithVerticalRetrace = value;
	}

	public static bool MSAA
	{
		get => Gdm.PreferMultiSampling;
		set => Gdm.PreferMultiSampling = value;
	}

	public static void Apply()
	{
		Gdm.ApplyChanges();
		if (_currentWidth != Width || _currentHeight != Height || _currentFullscreen != IsFullscreen)
		{
			_currentWidth = Width;
			_currentHeight = Height;
			_currentFullscreen = IsFullscreen;
			OnScreenSizeChanged?.Invoke();
		}

		if (!_applied)
		{
			_applied = true;
			Game.Window.ClientSizeChanged += (_, _) => OnScreenSizeChanged?.Invoke();
		}
	}

	public static Point GetDisplayDimensions()
	{
		// todo
		// Microsoft.Win32.Registry package
		return new Point(
			Gdm.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
			Gdm.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
	}

	public static void SetWindowMode(WindowModes mode)
	{
		if (DateTime.Now.Subtract(_dateSetWindowMode) < TimeSpan.FromSeconds(0.5))
		{
			return;
		}

		_dateSetWindowMode = DateTime.Now;

		var graphics = Gdm;
		var window = Game.Window;
		Point res;

		_windowMode = mode;

		switch (_windowMode)
		{
			case WindowModes.Windowed:
			{
				res = new Point(_originalWidth, _originalHeight);
				IsFullscreen = false;
				graphics.HardwareModeSwitch = false;
				Apply();

				var displayDims = GetDisplayDimensions();
				window.Position = new Point(displayDims.X / 2 - res.X / 2, displayDims.Y / 2 - res.Y / 2);
				window.IsBorderless = false;
				break;
			}
			case WindowModes.Borderless:
			{
				_originalWidth = Width;
				_originalHeight = Height;
				res = GetDisplayDimensions();
				graphics.HardwareModeSwitch = false;
				IsFullscreen = false;
				Apply();
				window.Position = Point.Zero;
				window.IsBorderless = false;
				break;
			}
			case WindowModes.Fullscreen:
			{
				_originalWidth = Width;
				_originalHeight = Height;
				res = GetDisplayDimensions();
				IsFullscreen = true;
				graphics.HardwareModeSwitch = true;
				Apply();
				window.Position = Point.Zero;
				window.IsBorderless = false;
				break;
			}
			default:
				res = new Point(1280, 720);
				break;
		}

		SetResolution(res.X, res.Y);
	}

	public static bool ToggleFullscreen()
	{
		if (DateTime.Now.Subtract(_dateToggleFullscreen) < TimeSpan.FromSeconds(0.5))
		{
			return false;
		}

		_dateToggleFullscreen = DateTime.Now;

		if (_windowMode == WindowModes.Fullscreen)
		{
			SetWindowMode(WindowModes.Windowed);
		}
		else
		{
			SetWindowMode(WindowModes.Fullscreen);
		}

		return true;
	}

	public static void SetResolution(int w, int h)
	{
		Width = w;
		Height = h;
		Apply();
	}

	public static void CenterScreen()
	{
		var window = Game.Window;
		var displayDims = GetDisplayDimensions();
		window.Position = new Point(displayDims.X / 2 - Width / 2, displayDims.Y / 2 - Height / 2);
	}
}