using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Assets;

public class Content
{
	public static readonly Content Get = new Content();

	public AtlasRegion Pixel;

	// public SpriteFont DefaultFont;
	public DynamicSpriteFont DefaultFont;
	public FontSystem FontSystem;

	private Dictionary<string, Texture2D> _textures;
	private Dictionary<string, AtlasRegion> _textureRegions;
	private Dictionary<string, SpriteAtlas> _atlases;

	public Microsoft.Xna.Framework.Content.ContentManager Manager;
	private GraphicsDevice _gd;

	public static string TexturePath = @"textures/";
	public static string SoundPath = @"sound/";
	public static string FontPath = @"fonts/";
	public static string ShaderPath = @"shaders/";

	public void Init(FortGame game, ContentConfig? config = null)
	{
		Manager = game.Content;
		_gd = game.GraphicsDevice;

		_textures = new Dictionary<string, Texture2D>();
		_textureRegions = new Dictionary<string, AtlasRegion>();
		_atlases = new Dictionary<string, SpriteAtlas>();

		SetConfig(config);
	}

	public void SetConfig(ContentConfig cfg)
	{
		if (cfg == null || cfg.Pixel == null)
		{
			var pixel = new Texture2D(_gd, 1, 1);
			pixel.SetData(new Color[] { Color.White });
			Pixel = new AtlasRegion(pixel, new Rectangle(0, 0, 1, 1));
		}
		else
		{
			Pixel = new AtlasRegion(cfg.Pixel,
				new Rectangle(cfg.Pixel.Frame.X, cfg.Pixel.Frame.Y, 1, 1));
		}

		if (cfg?.DefaultFont == null)
		{
			FontSystem = new FontSystem();
			FontSystem.AddFont(TitleContainer.OpenStream("content/fonts/defaultfont.ttf"));
		}
		else
		{
			FontSystem = cfg.DefaultFont;
		}

		DefaultFont = FontSystem.GetFont(18f);
	}

	public SpriteAtlas LoadSpriteAtlas(SpriteAtlasData data)
	{
		string path = Path.Combine(TexturePath, data.name.Replace('-', '_'));
		string name = data.name.Split('\\').Last().Split('.').First();

		//  get tx
		if (!_textures.TryGetValue(name, out var tx))
			tx = Manager.Load<Texture2D>(path);

		//  add tx to texture set
		_textures.Add(name, tx);

		//  create & add atlas
		var atlas = new SpriteAtlas(name, tx, data.sprites);
		_atlases.Add(name, atlas);

		//  add all regions of atlas
		for (int i = 0; i < data.sprites.Count; i++)
		{
			var spr = data.sprites[i];
			_textureRegions.Add(spr.name.Replace('-', '_'),
				new AtlasRegion(tx, spr.frame.X, spr.frame.Y, spr.frame.Width, spr.frame.Height));
		}

		//  yes
		return atlas;
	}

	public Texture2D LoadTexture(string path, string name)
	{
		var tx = Manager.Load<Texture2D>(path);
		_textures.Add(name, tx);
		return tx;
	}

	public Texture2D GetTexture(string name)
	{
		return _textures[name];
	}

	public AtlasRegion GetRegion(string name)
	{
		return _textureRegions[name];
	}
}