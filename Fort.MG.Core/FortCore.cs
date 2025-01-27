using Microsoft.Xna.Framework;

namespace Fort.MG.Core;

public static class FortCore
{
	public static Game Game { get; private set; }

	public static event EventHandler<EventArgs>? WindowSizeChanged;

	public static void Start(Game game)
	{
		Game = game;
		Graphics.Start(game);
	}

	public static void Init()
	{
		Game.Window.ClientSizeChanged += WindowOnClientSizeChanged;
		Graphics.Init();
	}

	public static void Update(GameTime gt)
	{
		Input.Update();
	}

	public static void PostUpdate(GameTime gt)
	{
		Input.PostUpdate();
	}

	private static void WindowOnClientSizeChanged(object? sender, EventArgs e)
	{
		WindowSizeChanged?.Invoke(sender, e);
	}
}
