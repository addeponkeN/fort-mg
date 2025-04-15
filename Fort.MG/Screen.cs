using Microsoft.Xna.Framework;

namespace Fort.MG;

public static class Screen
{
	private static GraphicsDeviceManager Gd => Graphics.GDM;
	private static Game Game => FortCore.Game;

	public static event Action OnScreenSizeChanged;

	public static Rectangle Bounds => new Rectangle(0, 0, Width, Height);

	private static bool _applied = false;

	private static int _currentWidth;
	private static int _currentHeight;
	private static bool _currentFullscreen;

	public static int Width
	{
		get => Gd.PreferredBackBufferWidth;
		set => Gd.PreferredBackBufferWidth = value;
	}

	public static int Height
	{
		get => Gd.PreferredBackBufferHeight;
		set => Gd.PreferredBackBufferHeight = value;
	}

	public static bool IsFullscreen
	{
		get => Gd.IsFullScreen;
		set => Gd.IsFullScreen = value;
	}

	public static bool VSync
	{
		get => Gd.SynchronizeWithVerticalRetrace;
		set => Gd.SynchronizeWithVerticalRetrace = value;
	}

	public static bool MSAA
	{
		get => Gd.PreferMultiSampling;
		set => Gd.PreferMultiSampling = value;
	}

	public static void Apply()
	{
		Gd.ApplyChanges();
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
}