using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FontStashSharp;
using Fort.MG.Assets.Data;
using Fort.MG.Assets.Storage;
using Fort.MG.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets;

public class AssetManager
{
	private Dictionary<Type, IAssetStorage> Storage
	{
		get
		{
			if (_storage == null)
			{
				_storage = new();
				RegisterBaseStorage();
			}
			return _storage;
		}
	}

	internal static string FullContentPath;
	internal static Microsoft.Xna.Framework.Content.ContentManager ContentManager;

	public SpriteRegion Pixel;
	public DynamicSpriteFont DefaultFont;
	public FontSystem DefaultFontSystem;
	private Dictionary<Type, IAssetStorage> _storage;

	public AssetManager(AssetConfig? config = null)
	{
		ContentManager = FortCore.Game.Content;
		FullContentPath = Path.Combine($"{Directory.GetCurrentDirectory()}", ContentManager.RootDirectory);

		SetConfig(config);
	}

	private void RegisterBaseStorage()
	{
		RegisterStorage(new TextureStorage());
		RegisterStorage(new SoundStorage());
		RegisterStorage(new FontStorage());
		RegisterStorage(new SpriteAtlasStorage());
	}

	public void RegisterStorage<T>(BaseStorage<T> storage)
	{
		var t = typeof(T);
		Storage.Add(t, storage);
	}

	public T GetStorage<T>() where T : IAssetStorage
	{
		return (T)Storage.Values.Where(x => x is T);
	}

	public T GetAsset<T>(string name)
	{
		var t = typeof(T);
		return (T)Storage[t].Get(name);
	}

	private void SetConfig(AssetConfig cfg)
	{
		if (cfg == null || cfg.Pixel == null)
		{
			var pixel = new Texture2D(Graphics.GraphicsDevice, 1, 1);
			pixel.SetData([Color.White]);
			Pixel = new SpriteRegion("pixel", pixel, new Rectangle(0, 0, 1, 1));
		}
		else
		{
			Pixel = new SpriteRegion("pixel", cfg.Pixel.Texture,
				new Rectangle(cfg.Pixel.Frame.X, cfg.Pixel.Frame.Y, 1, 1));
		}

		if (cfg?.DefaultFont == null)
		{
			DefaultFontSystem = new FontSystem();
			DefaultFontSystem.AddFont(TitleContainer.OpenStream("content/fonts/defaultfont.ttf"));
		}
		else
		{
			DefaultFontSystem = cfg.DefaultFont;
		}

		DefaultFont = DefaultFontSystem.GetFont(18f);
	}
}