using Fort.MG.EntitySystem;
using Fort.MG.Gui;
using Fort.MG.Systems;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Camera = Fort.MG.EntitySystem.Components.Camera;

namespace Fort.MG.Scenes;

public enum SceneStates
{
	Active,
	Entering,
	Exiting,
	Exited,
}

public partial class Scene
{
	private RenderTarget2D _target;
	private FortTimer _enterTime;
	private FortTimer _exitTime;

	internal SceneStates State;

	internal SceneManager SceneManager;
	public EngineSystemManager SystemManager;
	public EntityManager EntityManagerSystem;
	internal CanvasSystem CanvasSystem;

	public Camera Cam;

	public Vector2 MousePosition;
	public Vector2 MousePositionWorld;

	public SpriteSortMode SortMode;
	public BlendState BlendState;
	public SamplerState SamplerState;
	public DepthStencilState Stencil;
	public RasterizerState Rasterizer;
	public Effect Effect;

	public Canvas Canvas => CanvasSystem.Canvas;

	protected bool InitedFirstFrame;

	internal bool IsLoaded;
	internal bool IsInited;

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

	public virtual void Init()
	{
		IsInited = true;
		SystemManager = new EngineSystemManager();
		SystemManager.Add(EntityManagerSystem = new EntityManager(new BasicEntityCollection()));
		SystemManager.Add(CanvasSystem = new CanvasSystem());

		Cam = Entity.Create<Camera>();
		EntityManagerSystem.Add(Cam.Entity);

		EnterTime = 0.1f;
		ExitTime = 0.1f;

		_target = new RenderTarget2D(Graphics.GraphicsDevice, Cam.Viewport.Width, Cam.Viewport.Height,
			false, SurfaceFormat.Color, DepthFormat.Depth24);

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

	public void SetScene(Scene scene) => SceneManager.SetScene(scene);

	internal void SetState(SceneStates sceneState)
	{
		this.State = sceneState;
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

	public virtual void Update(IGameTime t)
	{
		MousePosition = Input.MousePos;
		MousePositionWorld = Input.MouseTransformedPos(Cam.UpdateMatrix);
		if (!InitedFirstFrame)
		{
			Start();
			InitedFirstFrame = true;
		}
		SystemManager.Update(t);
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

	public virtual void Render()
	{
		SystemManager.Render();
	}

	public virtual void Draw()
	{
		var gd = Graphics.GraphicsDevice;
		var sb = Graphics.SpriteBatch;
		gd.SetRenderTarget(_target);
		gd.Clear(new Color(25, 25, 25));

		sb.Begin(SortMode, BlendState, SamplerState, Stencil, Rasterizer, Effect, Cam.DrawMatrix);
		SystemManager.Draw();
		sb.End();

		float lerp = State == SceneStates.Active ? 0f :
			State == SceneStates.Entering ? EnterTime.LerpReverse :
			ExitTime.Lerp;

		//  screen enter/exit fade in/out
		if (State != SceneStates.Active)
		{
			sb.Begin();
			sb.Draw(FortEngine.AssetManager.Pixel, new Rectangle(0, 0, Cam.Viewport.Width, Cam.Viewport.Height),
				FortEngine.AssetManager.Pixel, Color.Black * lerp);
			sb.End();
		}

		gd.SetRenderTarget(null);
		gd.Clear(new Color(25, 25, 25));

		sb.Begin();
		sb.Draw(_target, Screen.Bounds, Color.White);
		sb.End();

		SystemManager.DrawGui();
	}

	public virtual void Exit()
	{
		SetState(SceneStates.Exiting);
	}
}