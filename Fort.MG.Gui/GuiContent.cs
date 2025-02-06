using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

internal static class GuiContent
{
	internal static bool IsLoaded { get; private set; }

	private static string _contentRoot;
	private static Dictionary<string, FontSystem> _fonts = new();

	internal static FontSystem DefaultFontSystem { get; private set; }
	internal static Texture2D Pixel { get; private set; }

	internal static void Load()
	{
		if (IsLoaded)
			return;
		IsLoaded = true;

		_contentRoot = Path.Combine(Directory.GetCurrentDirectory(), FortCore.Game.Content.RootDirectory, "fonts");
		Pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
		Pixel.SetData([Color.White]);
		_fonts = new Dictionary<string, FontSystem>();

		TryLoadDefaultFont();

		GC.Collect();
	}

	internal static void Dispose()
	{
		Pixel.Dispose();
		foreach (var fontSystem in _fonts)
			fontSystem.Value.Dispose();
		_fonts.Clear();
		DefaultFontSystem = null;
	}
	internal static DynamicSpriteFont GetDefaultFont(float size = 16f) => DefaultFontSystem.GetFont(size);

	private static void TryLoadDefaultFont()
	{
		if (DefaultFontSystem != null) return;

		string path = Path.Combine(_contentRoot, "defaultfont.ttf");
		if (File.Exists(path))
		{
			DefaultFontSystem = LoadFont(path);
		}
	}

	internal static FontSystem LoadFont(string path)
	{
		path = Path.Combine(_contentRoot, path);
		var name = Path.GetFileNameWithoutExtension(path);

		var sys = new FontSystem(new FontSystemSettings
		{
			FontResolutionFactor = 2f,
		});

		sys.AddFont(File.ReadAllBytes(path));
		_fonts.Add(name, sys);

		return sys;
	}

	internal static DynamicSpriteFont GetDefaultFont()
	{
		return DefaultFontSystem.GetFont(16f);
	}

	internal static DynamicSpriteFont GetFont(string name, float size = 16f)
	{
		return _fonts[name].GetFont(size);
	}

	internal static string[] GetFontNames => _fonts.Keys.ToArray();

}
