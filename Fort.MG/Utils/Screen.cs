using Microsoft.Xna.Framework;

namespace Fort.MG.Utils;

public static class Screen
{
	private static GraphicsDeviceManager gd;

    public static void Init(GraphicsDeviceManager gfxDeviceManager)
    {
        gd = gfxDeviceManager;
    }

    public static Rectangle Bounds => new Rectangle(0, 0, Width, Height);
    
    public static int Width
    {
        get => gd.PreferredBackBufferWidth;
        set => gd.PreferredBackBufferWidth = value;
    }

    public static int Height
    {
        get => gd.PreferredBackBufferHeight;
        set => gd.PreferredBackBufferHeight = value;
    }

    public static bool IsFullscreen
    {
        get => gd.IsFullScreen;
        set => gd.IsFullScreen = value;
    }

    public static bool VSync
    {
        get => gd.SynchronizeWithVerticalRetrace;
        set => gd.SynchronizeWithVerticalRetrace = value;
    }

    public static bool MSAA
    {
        get => gd.PreferMultiSampling;
        set => gd.PreferMultiSampling = value;
    }

    public static void Apply() => gd.ApplyChanges();
}