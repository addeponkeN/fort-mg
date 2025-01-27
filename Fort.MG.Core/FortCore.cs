using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Core;

public static class FortCore
{
	public static GraphicsDevice GraphicsDevice;
	public static Game Game;

	public static void Init(Game game)
	{
		Game = game;
		GraphicsDevice = game.GraphicsDevice;
		Input.Init(Game);
	}

	public static void Update(GameTime gt)
	{
		Input.Update();
	}

	public static void PostUpdate(GameTime gt)
	{
		Input.PostUpdate();
	}
}
