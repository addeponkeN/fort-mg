using Microsoft.Xna.Framework;

namespace Fort.MG;

public static class FortCore
{
	public static Game Game { get; private set; }

	public static event EventHandler<EventArgs>? WindowSizeChanged;

	public static void Start(Game game)
	{
		Game = game ?? throw new ArgumentNullException(nameof(game));
		Graphics.Start();
	}

	public static void Init()
	{
		if (Game == null)
			throw new InvalidOperationException("Game instance is not set. Call Start() first.");

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

	public static void Dispose()
	{
		if (Game != null)
		{
			Game.Window.ClientSizeChanged -= WindowOnClientSizeChanged;
		}
		Graphics.Dispose();
	}
}