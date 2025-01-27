using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Core;

public static class Graphics
{

	private static Game _game;

	public static SpriteBatch SpriteBatch { get; private set; }
	public static GraphicsDevice GraphicsDevice { get; private set; }
	public static GraphicsDeviceManager GDM { get; private set; }

	internal static void Start(Game game)
	{
		_game = game;
		GDM = new GraphicsDeviceManager(_game);
	}

	internal static void Init()
	{
		GraphicsDevice = _game.GraphicsDevice;
		SpriteBatch = new SpriteBatch(GraphicsDevice);
	}
}