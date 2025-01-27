using Fort.MG.Assets;
using Fort.MG.Core;
using Fort.MG.EntitySystem;
using Fort.MG.Scenes;
using Fort.MG.Systems;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Camera = Fort.MG.EntitySystem.Components.Camera;

namespace Fort.MG;

public static class Engine
{
    public static FortGame Game;
    public static GameTime gt;
    public static SceneManager SceneManager;
    public static GraphicsDeviceManager GraphicsDeviceManager;

    /// <summary>
    /// get current scene
    /// </summary>
    public static Scene Scene => SceneManager.Scene;

    /// <summary>
    /// get camera from current scene
    /// </summary>
    public static Camera Cam => SceneManager.Scene.Cam;

    internal static EngineSystemManager SystemManager;

    public static Content Content;

    static bool _startedExiting;

    public static void Init(FortGame game)
    {
        Init(game, new DefaultGameTime());
    }
    
    public static void Init(FortGame game, IGameTime oboTime)
    {
        SceneManager = game.SceneManager;
        Game = game;
        Time.Init(oboTime);
        SystemManager = new EngineSystemManager();
        SystemManager.Add(Entity.Create<TimerSystem>());
        SystemManager.Add(Entity.Create<DebugPrinter>());
        Content = new Content();
        Content.Init(game);
        
        Input.Init(game);
    }

    public static void InitGraphics(SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
    {        
        GraphicsDeviceManager = graphics;
        Graphics.Init(Game._spriteBatch);
    }

    public static void RegisterSystem<T>() where T : EngineSystem, new()
    {
        SystemManager.Add(Entity.Create<T>());
    }

    public static T GetSystem<T>() where T : EngineSystem => SystemManager.Get<T>();

    internal static void Clear()
    {
        SystemManager.Clear();
    }

    internal static void FirstFrameInit(GameTime gameTime)
    {
        gt = gameTime;
        // Time = new Time(gt);
    }

    internal static void Update()
    {
        if(_startedExiting)
        {
            Game.Exit();
            return;
        }
        Input.Update();
        SystemManager.Update(Time.GetTimeManager());
    }

    internal static void PostUpdate()
    {
        SystemManager.PostUpdate(Time.GetTimeManager());
        Input.PostUpdate();
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
        Graphics.SpriteBatch.End();
    }

    public static void Exit()
    {
        _startedExiting = true;
    }
}