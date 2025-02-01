using Microsoft.Xna.Framework;

namespace Fort.MG.Core;

public static class Screen
{
	private static GraphicsDeviceManager Gd => Graphics.GDM;

	public static Rectangle Bounds => new Rectangle(0, 0, Width, Height);

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

	public static void Apply() => Gd.ApplyChanges();

}