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
		_game = game ?? throw new ArgumentNullException(nameof(game));
		GDM = new GraphicsDeviceManager(_game);
	}

	internal static void Init()
	{
		if (_game == null)
			throw new InvalidOperationException("Game instance is not set. Call Start() first.");

		GraphicsDevice = _game.GraphicsDevice;
		SpriteBatch = new SpriteBatch(GraphicsDevice);
	}

	public static void Dispose()
	{
		SpriteBatch?.Dispose();
		GDM?.Dispose();
	}
}