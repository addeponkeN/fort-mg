using FontStashSharp;
using Fort.MG.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG;

//public class ContentManager
//{
//	private readonly string _contentRoot;
//	internal Texture2D Pixel;

//	private Dictionary<string, FontSystem> _fonts = new();

//	public FontSystem DefaultFont { get; private set; }

//	public ContentManager()
//	{
//		_contentRoot = Path.Combine(Directory.GetCurrentDirectory(), FortCore.Game.Content.RootDirectory, "fonts");
//		Pixel = new Texture2D(FortCore.GraphicsDevice, 1, 1);
//		Pixel.SetData([Color.White]);
//		_fonts = new Dictionary<string, FontSystem>();

//		TryLoadDefaultFont();

//		GC.Collect();
//	}

//	private void TryLoadDefaultFont()
//	{
//		string path = Path.Combine(_contentRoot, "defaultfont.ttf");
//		if (File.Exists(path))
//		{
//			DefaultFont = LoadFont(path);
//		}
//	}

//	public FontSystem LoadFont(string path)
//	{
//		path = Path.Combine(_contentRoot, path);
//		var name = Path.GetFileNameWithoutExtension(path);

//		var sys = new FontSystem(new FontSystemSettings
//		{
//			FontResolutionFactor = 2f,
//		});

//		sys.AddFont(File.ReadAllBytes(path));
//		_fonts.Add(name, sys);

//		return sys;
//	}

//	public DynamicSpriteFont GetDefaultFont()
//	{
//		return DefaultFont.GetFont(16f);
//	}

//	public DynamicSpriteFont GetFont(string name, float size = 16f)
//	{
//		return _fonts[name].GetFont(size);
//	}

//	public string[] GetFontNames => _fonts.Keys.ToArray();

//}
