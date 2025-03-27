using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Gui;

public static class GuiContent
{
	internal static bool IsLoaded { get; private set; }

	private static string _contentRoot;
	private static Dictionary<string, FontSystem> _fonts = new();

	internal static FontSystem DefaultFontSystem { get; private set; }
	internal static Texture2D Pixel { get; private set; }
	internal static Texture2D PixelSource { get; private set; }

	internal static Dictionary<Texture2D, Texture2D> _whites = new();

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

	public static Texture2D GetWhiteTexture(Texture2D tx)
	{
		if (tx == null) return null;
		if (tx == Pixel) return Pixel;

		if (_whites.TryGetValue(tx, out var txWhite))
			return txWhite;

		txWhite = CreateWhiteTexture(tx);

		return txWhite;
	}

	private static Texture2D CreateWhiteTexture(Texture2D tx)
	{
		var data = new Color[tx.Width * tx.Height];
		var txData = new Color[data.Length];
		tx.GetData(txData);
		for (int i = 0; i < txData.Length; i++)
		{
			var c = txData[i];
			if (c.A == 0) 
				continue;
			if (c is { R: < 1, G: < 1, B: < 1 })
				continue;
			data[i] = Color.White;
		}
		var white = new Texture2D(Graphics.GraphicsDevice, tx.Width, tx.Height);
		white.SetData(data);

		_whites.Add(tx, white);
		return white;
	}

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
