using Fort.MG.Scenes;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG;

public class FortGame : Game
{
    public event EventHandler<EventArgs> WindowSizeChanged;

    protected GraphicsDeviceManager graphicsManager;
    internal SpriteBatch _spriteBatch;

    private bool isFirstFrame;

    public SceneManager SceneManager;

    public FortGame()
    {
        graphicsManager = new GraphicsDeviceManager(this);

        Content.RootDirectory = "content";

        Window.AllowAltF4 = true;
        IsMouseVisible = true;
        IsFixedTimeStep = false;

        LimitFPS(200);

        Screen.Init(graphicsManager);
        SceneManager = new SceneManager();
    }

    public void LimitFPS(int limit)
    {
        TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / limit);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Screen.Width = 1920;
        Screen.Height = 1080;
        Screen.MSAA = true;
        Screen.VSync = true;
        Screen.Apply();

        Engine.Init(this);
        Engine.InitGraphics(_spriteBatch, graphicsManager);
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    private void OnFirstFrame(GameTime gt)
    {
        isFirstFrame = true;
        Engine.FirstFrameInit(gt);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Engine.gt = gameTime;
        if(!isFirstFrame)
            OnFirstFrame(gameTime);

        Engine.Update();

        SceneManager.Update(Time.GetTimeManager());

        Engine.PostUpdate();
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        Engine.DrawBegin();
        SceneManager.Render();
        SceneManager.Draw();
        Engine.Draw();
        Engine.DrawEnd();
    }
}