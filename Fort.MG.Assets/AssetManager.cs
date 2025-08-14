using FontStashSharp;
using Fort.MG.Assets.Data;
using Fort.MG.Assets.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets;

public class AssetManager : IDisposable
{
	private readonly Dictionary<Type, object> _storage = new();

	public static string FullContentPath { get; private set; }
	public static string ContentRoot { get; private set; }
	public static ContentManager ContentManager { get; private set; }

	public SpriteRegion Pixel { get; private set; }
	public DynamicSpriteFont DefaultFont { get; private set; }
	public FontSystem DefaultFontSystem { get; private set; }

	public AssetManager(AssetConfig? config = null)
	{
		ContentManager = FortCore.Game.Content;
		ContentRoot = ContentManager.RootDirectory;
		FullContentPath = Path.Combine(Directory.GetCurrentDirectory(), ContentRoot);
		RegisterBaseStorage();
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
		_storage[typeof(T)] = storage;
	}

	public BaseStorage<T> GetStorage<T>()
	{
		if (_storage.TryGetValue(typeof(T), out var store))
			return (BaseStorage<T>)store;

		throw new KeyNotFoundException($"No storage registered for type {typeof(T).Name}");
	}

	public T LoadAsset<T>(string name)
	{
		return GetStorage<T>().Load(name);
	}

	public T GetAsset<T>(string name) => GetStorage<T>().Get(name);

	private void SetConfig(AssetConfig? cfg)
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

	public void Dispose()
	{
		foreach (var store in _storage.Values)
			if (store is IDisposable disposable)
				disposable.Dispose();
		_storage.Clear();
	}
}
