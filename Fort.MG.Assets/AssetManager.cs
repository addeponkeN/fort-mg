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
		RegisterStorage(new SpriteAtlasStorage(GetStorage<TextureStorage>()));
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

	//	public SpriteAtlas LoadSpriteAtlas(SpriteAtlasData data)
	//	{
	//		string texturePath = Path.Combine(TexturePath, data.name.Replace('-', '_'));
	//		string atlasName = data.name.Split('\\').Last().Split('.').First();

	//		//  get tx
	//		if (!_textures.TryGetValue(atlasName, out var tx))
	//			tx = ContentManager.Load<Texture2D>(texturePath);

	//		//  add tx to texture set
	//		_textures.Add(atlasName, tx);

	//		var regions = data.sprites.Select(x => new SpriteRegion(x.name, tx, x.frame.X, x.frame.Y, x.frame.Width, x.frame.Height))
	//			.ToList();

	//		//  create & add atlas
	//		var atlas = new SpriteAtlas(atlasName, tx, regions);
	//		_atlases.Add(atlasName, atlas);

	//		//  add all regions of atlas
	//		foreach (var spr in data.sprites)
	//		{
	//			var sprName = spr.name.Replace('-', '_');
	//			_regions.Add(sprName,
	//				new SpriteRegion(sprName, tx, spr.frame.X, spr.frame.Y, spr.frame.Width, spr.frame.Height));
	//		}

	//		//  yes
	//		return atlas;
	//	}

	//	public Texture2D LoadTexture(string path, string name)
	//	{
	//		var tx = ContentManager.Load<Texture2D>(path);
	//		_textures.Add(name, tx);
	//		return tx;
	//	}

	//	public Texture2D GetTexture(string name)
	//	{
	//		return _textures[name];
	//	}

	//	public SpriteRegion GetRegion(string name)
	//	{
	//		return _regions[name];
	//	}


}