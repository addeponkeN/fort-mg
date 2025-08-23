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
	private readonly Dictionary<string, SpriteRegion> _spriteRegions = new();
	private readonly Dictionary<TileRegionKey, TilesetRegion> _tileRegions = new();

	public static string FullContentPath { get; private set; }
	public static string ContentRoot { get; private set; }
	public static ContentManager ContentManager { get; private set; }

	public SpriteRegion Pixel { get; private set; }
	public DynamicSpriteFont DefaultFont { get; private set; }
	public FontSystem DefaultFontSystem { get; private set; }

	private SpriteRegion _unknownSpriteRegion;

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
		RegisterStorage(new TileAtlasStorage());
	}

	public void RegisterStorage<T>(BaseStorage<T> storage)
	{
		storage.AssetManager = this;
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

	public TilesetRegion GetTile(string tileset, int orientation, int variant = 0)
	{
		var key = new TileRegionKey(tileset, orientation, variant);
		return _tileRegions[key];
	}

	public SpriteRegion GetSprite(string name)
	{
		return _spriteRegions.GetValueOrDefault(name, _unknownSpriteRegion);
	}

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

		var unknownTexture = CreateUnknownTexture(Graphics.GraphicsDevice);
		_unknownSpriteRegion = new SpriteRegion("unknown", unknownTexture, new Rectangle(0, 0, unknownTexture.Width, unknownTexture.Height));
	}

	public void Dispose()
	{
		foreach (var store in _storage.Values)
			if (store is IDisposable disposable)
				disposable.Dispose();
		_storage.Clear();
	}

	internal void RegisterTileRegion(TilesetRegion region)
	{
		var key = new TileRegionKey(region.Tileset, region.Orientation, region.Variant);
		_tileRegions.TryAdd(key, region);
	}

	internal void RegisterSpriteRegion(SpriteRegion region)
	{
		if (!_spriteRegions.TryAdd(region.Name, region))
		{
			string name = $"{region.AtlasName}-{region.Name}";
			_spriteRegions.Add(name, region);
		}
	}

	private Texture2D CreateUnknownTexture(GraphicsDevice graphics)
	{
		int size = 64;
		Texture2D tex = new Texture2D(graphics, size, size);

		Color color1 = Color.Magenta;
		Color color2 = Color.Black;

		Color[] data = new Color[size * size];

		int tileSize = 8;

		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				bool isEvenTile = ((x / tileSize) + (y / tileSize)) % 2 == 0;
				data[y * size + x] = isEvenTile ? color1 : color2;
			}
		}

		tex.SetData(data);
		return tex;
	}
}
