using Fort.MG.EntitySystem;
using Fort.MG.Gui;
using Fort.MG.Rendering;
using Fort.MG.Systems;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Camera = Fort.MG.Components.Camera;

namespace Fort.MG.Scenes;

public enum SceneStates
{
    Active,
    Entering,
    Exiting,
    Exited,
}

public class Scene : BaseScene
{
    private FortTimer _enterTime;
    private FortTimer _exitTime;

    private RenderTarget2D _mainTarget;

    internal readonly RenderPassManager RenderPassManager = new RenderPassManager();
    internal readonly List<PooledRenderPassBucket> BucketsBuffer = new List<PooledRenderPassBucket>(16);

    public static Scene Current => SceneManager.CurrentScene;

    public Camera Cam { get; private set; }
    public EntityManager EntityManagerSystem { get; private set; }

    internal SceneManager SceneManager { get; set; }
    internal CanvasSystem CanvasSystem { get; private set; }

    public FortTimer EnterTime
    {
        get => _enterTime;
        set
        {
            _enterTime = value;
            _enterTime.OnTick += () => SetState(SceneStates.Active);
        }
    }

    public FortTimer ExitTime
    {
        get => _exitTime;
        set
        {
            _exitTime = value;
            _exitTime.OnTick += () => SetState(SceneStates.Exited);
        }
    }

    public Canvas Canvas => CanvasSystem.Canvas;

    public Vector2 MousePosition { get; private set; }
    public Vector2 MousePositionWorld { get; private set; }

    public override void Init()
    {
        EnterTime = 0.1f;
        ExitTime = 0.1f;

        base.Init();

        SceneSystemManager.Add(CanvasSystem = new CanvasSystem());
        SceneSystemManager.Add(EntityManagerSystem = new EntityManager(new BasicEntityCollection()));

        Cam = Entity.Create<Camera>();
        EntityManagerSystem.Add(Cam.Entity);

        _mainTarget = new RenderTarget2D(Graphics.GraphicsDevice, Screen.Width, Screen.Height, false, SurfaceFormat.Color, DepthFormat.Depth24);
    }

    public void SetScene(Scene scene) => SceneManager.SetScene(scene);

    internal override void SetState(SceneStates sceneState)
    {
        base.SetState(sceneState);
        switch (sceneState)
        {
            case SceneStates.Entering:
                if (!EnterTime.IsRunning)
                    EnterTime.Start();
                break;
            case SceneStates.Exiting:
                if (!ExitTime.IsRunning)
                    ExitTime.Start();
                break;
        }
    }


    public override void Update(IGameContext t)
    {
        MousePosition = Input.MousePos;
        MousePositionWorld = Input.MouseTransformedPos(Cam.DrawMatrix);

        base.Update(t);
    }

    public virtual void Render()
    {
        Canvas.Render();
        SceneSystemManager.Render();
    }

    private void PreDrawUpdate()
    {
        RenderPasses.AlphaTestEffectDefault.Projection =
            Scene.Current.Cam.DrawMatrix *
            Matrix.CreateOrthographicOffCenter(0, Screen.Width, Screen.Height, 0, 0, -1);
    }

    public virtual void Draw()
    {
        PreDrawUpdate();

        var gd = Graphics.GraphicsDevice;
        var sb = Graphics.SpriteBatch;
        gd.SetRenderTarget(_mainTarget);
        gd.Clear(new Color(25, 25, 25));

        // In Draw():
        var renderables = EntityManagerSystem.GetRenderables();
        RenderPassManager.CollectIntoBuckets(renderables, BucketsBuffer);

        for (int bi = 0; bi < BucketsBuffer.Count; bi++)
        {
            var bucket = BucketsBuffer[bi];
            if (bucket.Renderables.Count == 0)
                continue;

            var pass = bucket.Pass;
            sb.Begin(pass.SortMode, pass.BlendState, pass.SamplerState,
                pass.DepthStencilState, pass.RasterizerState, pass.Effect, Cam.DrawMatrix);

            var list = bucket.Renderables;
            for (int i = 0, n = list.Count; i < n; i++)
                list[i].Draw();

            sb.End();
        }

        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, null, null, null, Cam.DrawMatrix);
        EntityManagerSystem.DrawGizmos();
        sb.End();

        if (State != SceneStates.Active)
        {
            float lerp = State == SceneStates.Active ? 0f :
                State == SceneStates.Entering ? EnterTime.LerpReverse :
                ExitTime.Lerp;

            sb.Begin();
            sb.Draw(FortEngine.Assets.Pixel, new Rectangle(0, 0, Cam.Viewport.Width, Cam.Viewport.Height),
                FortEngine.Assets.Pixel, Color.Black * lerp);
            sb.End();
        }

        gd.SetRenderTarget(null);
        gd.Clear(new Color(25, 25, 25));

        sb.Begin();
        sb.Draw(_mainTarget, Screen.Bounds, Color.White);
        sb.End();

        Canvas.Draw();

        SceneSystemManager.DrawGui();
    }
}

public class BaseScene
{
    internal SceneStates State;

    internal EngineSystemManager SceneSystemManager;

    protected bool InitedFirstFrame;

    internal bool IsLoaded;
    internal bool IsInited;

    public virtual void Init()
    {
        IsInited = true;
        SceneSystemManager = new EngineSystemManager();

        SetState(SceneStates.Entering);
    }

    public virtual void LoadContent()
    {
        IsLoaded = true;
    }

    /// <summary>
    /// before the first frame
    /// </summary>
    public virtual void Start()
    {
        InitedFirstFrame = true;
    }

    internal virtual void SetState(SceneStates sceneState)
    {
        this.State = sceneState;
    }

    public virtual void Update(IGameContext t)
    {

        if (!InitedFirstFrame)
        {
            Start();
            InitedFirstFrame = true;
        }
        SceneSystemManager.Update(t);
        switch (State)
        {
            case SceneStates.Active:
                ActiveUpdate();
                break;
        }
    }

    /// <summary>
    /// updates when the screen is done entering and is not exiting
    /// </summary>
    public virtual void ActiveUpdate()
    {
    }

    public virtual void Exit()
    {
        SetState(SceneStates.Exiting);
    }
}