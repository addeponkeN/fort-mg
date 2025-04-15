using Microsoft.Xna.Framework;

namespace Fort.MG;

public struct GameConfig
{
	public bool PreferHalfPixelOffset;
}

public class FortCoreGame : Game
{
	private bool _isFirstFrame;

	public FortCoreGame(GameConfig? gameConfig = null)
	{
		Content.RootDirectory = "content";

		Window.AllowAltF4 = true;
		IsMouseVisible = true;
		IsFixedTimeStep = true;

		FortCore.Start(this);

		if (gameConfig.HasValue)
		{
			var v = gameConfig.Value;
			Graphics.GDM.PreferHalfPixelOffset = v.PreferHalfPixelOffset;
		}
	}

	public void LimitFPS(int limit)
	{
		TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / limit);
	}

	protected override void Initialize()
	{
		LimitFPS(300);
		Screen.Width = 1920;
		Screen.Height = 1080;
		Screen.MSAA = true;
		Screen.VSync = true;
		Screen.Apply();

		FortCore.Init();

		base.Initialize();
	}

	protected override void LoadContent()
	{
		base.LoadContent();
	}

	protected virtual void OnFirstFrame(GameTime gt)
	{
		_isFirstFrame = true;
	}

	protected override void Update(GameTime gt)
	{
		if (!_isFirstFrame)
		{
			OnFirstFrame(gt);
		}

		FortCore.Update(gt);
		UpdateGame(gt);
		PostUpdateGame(gt);
	}

	protected virtual void UpdateGame(GameTime gt)
	{

	}

	protected virtual void PostUpdateGame(GameTime gt)
	{
		FortCore.PostUpdate(gt);
	}

	protected virtual void Render(GameTime gt)
	{
	}

	protected override void Draw(GameTime gt)
	{
		Render(gt);
	}
}