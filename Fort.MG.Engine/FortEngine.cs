using Fort.MG.Assets;
using Fort.MG.Components;
using Fort.MG.Extensions;
using Fort.MG.Scenes;
using Fort.MG.Systems;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG;

public static class FortEngine
{
	/// <summary>
	/// The default (and, today, only) game instance. Prefer the FortEngine.* static members below
	/// for existing call sites; Default exists so this state is owned by an explicit,
	/// instantiable object rather than bare static fields (see AI_CONTEXT.md §12).
	/// </summary>
	public static GameInstance Default { get; } = new();

	public static FortGame Game => Default.Game;

	public static GameTime Time
	{
		get => Default.Time;
		internal set => Default.Time = value;
	}

	public static SceneManager SceneManager => Default.SceneManager;

	/// <summary>
	/// get camera from current scene
	/// </summary>
	public static Camera Cam => SceneManager.Scene.Cam;

	internal static EngineSystemManager SystemManager
	{
		get => Default.SystemManager;
		set => Default.SystemManager = value;
	}

	public static AssetManager Assets => Default.Assets;

	internal static SceneManager CreateSceneManager()
	{
		return new SceneManager(SystemManager);
	}

	public static void Start(FortGame game)
	{
		Default.Game = game;
	}

	public static void Load()
	{
		Load(new DefaultGameContext());
	}

	public static void Load(IGameContext gameContext)
	{
		Default.SceneManager = Game.SceneManager;

		Utils.Time.Init(gameContext);
		FortExtensions.Initialize(Graphics.SpriteBatch);
		Default.SystemManager = new EngineSystemManager();

		SystemManager.Register<TimerSystem>();
		//SystemManager.Register<DebugPrinter>();
		SystemManager.Register<PerformanceMetricsSystem>();
		SystemManager.Register<SystemMessageSystem>();
		Default.Assets = new AssetManager();
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
	}

	internal static void PreUpdate()
	{
		SystemManager.PreUpdate(Utils.Time.GetTimeManager());
	}

	internal static void Update()
	{
		if (Default.StartedExiting)
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
		Default.StartedExiting = true;
	}
}