using Fort.MG.Assets;
using Fort.MG.EntitySystem.Components;
using Fort.MG.Extensions;
using Fort.MG.Scenes;
using Fort.MG.Systems;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG;

public static class FortEngine
{
	public static FortGame Game;
	public static GameTime Time;
	public static SceneManager SceneManager;

	/// <summary>
	/// get current scene
	/// </summary>
	public static Scene Scene => SceneManager.Scene;

	/// <summary>
	/// get camera from current scene
	/// </summary>
	public static Camera Cam => SceneManager.Scene.Cam;

	internal static EngineSystemManager SystemManager;

	public static AssetManager AssetManager;

	private static bool _startedExiting;

	internal static SceneManager CreateSceneManager()
	{
		return new SceneManager(SystemManager);
	}

	public static void Start(FortGame game)
	{
		Game = game;
	}

	public static void Load()
	{
		Load(new DefaultGameTime());
	}

	public static void Load(IGameTime gameTime)
	{
		SceneManager = Game.SceneManager;

		Utils.Time.Init(gameTime);
		FortExtensions.Initialize(Graphics.SpriteBatch);
		SystemManager = new EngineSystemManager();

		SystemManager.Register<TimerSystem>();
		SystemManager.Register<DebugPrinter>();
		AssetManager = new AssetManager();
	}

	public static void RegisterSystem<T>() where T : EngineSystem, new()
	{
		SystemManager.Register<T>();
	}

	public static T GetSystem<T>() where T : EngineSystem => SystemManager.Get<T>();

	internal static void Clear()
	{
		SystemManager.Clear();
	}

	internal static void FirstFrameInit(GameTime gameTime)
	{
		Time = gameTime;
		// Time = new Time(gt);
	}

	internal static void Update()
	{
		if (_startedExiting)
		{
			Game.Exit();
			return;
		}
		SystemManager.Update(Utils.Time.GetTimeManager());
	}

	internal static void PostUpdate()
	{
		SystemManager.PostUpdate(Utils.Time.GetTimeManager());
	}

	internal static void DrawBegin()
	{
		SystemManager.OnDrawBegin();
	}

	internal static void DrawEnd()
	{
		SystemManager.OnDrawEnd();
	}

	internal static void Draw()
	{
		Graphics.SpriteBatch.Begin();
		SystemManager.Draw();
		SystemManager.DrawGui();
		Graphics.SpriteBatch.End();
	}

	public static void Exit()
	{
		_startedExiting = true;
	}
}