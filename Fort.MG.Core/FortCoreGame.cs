using Microsoft.Xna.Framework;

namespace Fort.MG.Core;

public class FortCoreGame : Game
{
	private bool _isFirstFrame;

	public FortCoreGame()
	{
		Content.RootDirectory = "content";

		Window.AllowAltF4 = true;
		IsMouseVisible = true;
		IsFixedTimeStep = false;

		LimitFPS(200);

		FortCore.Start(this);
	}

	public void LimitFPS(int limit)
	{
		TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / limit);
	}

	protected override void Initialize()
	{
		FortCore.Init();

		Screen.Width = 1920;
		Screen.Height = 1080;
		Screen.MSAA = true;
		Screen.VSync = true;
		Screen.Apply();

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
