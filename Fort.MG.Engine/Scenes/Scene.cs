using Fort.MG.EntitySystem;
using Fort.MG.Gui;
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

public class Scene
{
	private RenderTarget2D _target;
	private FortTimer _enterTime;
	private FortTimer _exitTime;

	internal SceneStates State;

	internal SceneManager SceneManager;
	internal EngineSystemManager SceneSystemManager;
	internal CanvasSystem CanvasSystem;

	public EntityManager EntityManagerSystem;

	public Camera Cam;

	public Vector2 MousePosition { get; private set; }
	public Vector2 MousePositionWorld { get; private set; }

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
		SceneSystemManager = new EngineSystemManager();
		SceneSystemManager.Add(EntityManagerSystem = new EntityManager(new BasicEntityCollection()));
		SceneSystemManager.Add(CanvasSystem = new CanvasSystem());

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

	public virtual void Render()
	{
		SceneSystemManager.Render();
	}

	public virtual void Draw()
	{
		var gd = Graphics.GraphicsDevice;
		var sb = Graphics.SpriteBatch;
		gd.SetRenderTarget(_target);
		gd.Clear(new Color(25, 25, 25));

		sb.Begin(SortMode, BlendState, SamplerState, Stencil, Rasterizer, Effect, Cam.DrawMatrix);
		SceneSystemManager.Draw();
		sb.End();

		float lerp = State == SceneStates.Active ? 0f :
			State == SceneStates.Entering ? EnterTime.LerpReverse :
			ExitTime.Lerp;

		//  screen enter/exit fade in/out
		if (State != SceneStates.Active)
		{
			sb.Begin();
			sb.Draw(FortEngine.Assets.Pixel, new Rectangle(0, 0, Cam.Viewport.Width, Cam.Viewport.Height),
				FortEngine.Assets.Pixel, Color.Black * lerp);
			sb.End();
		}

		gd.SetRenderTarget(null);
		gd.Clear(new Color(25, 25, 25));

		sb.Begin();
		sb.Draw(_target, Screen.Bounds, Color.White);
		sb.End();

		SceneSystemManager.DrawGui();
	}

	public virtual void Exit()
	{
		SetState(SceneStates.Exiting);
	}
}