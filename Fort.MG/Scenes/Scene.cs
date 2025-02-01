using Fort.MG.Core;
using Fort.MG.Core.VirtualViewports;
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

public class Scene
{
	private RenderTarget2D _target;
	private FortTimer _enterTime;
	private FortTimer _exitTime;

	internal SceneStates State;

	public EngineSystemManager SystemManager;
	public EntityManager EntityManager;

	public Camera Cam;

	public Vector2 MousePosition;
	public Vector2 MousePositionWorld;

	public SpriteSortMode SortMode;
	public BlendState BlendState;
	public SamplerState SamplerState;
	public DepthStencilState Stencil;
	public RasterizerState Rasterizer;
	public Effect Effect;

	protected bool InitedFirstFrame;

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

	public Scene() : this(1280, 720) { }
	public Scene(int virtualWidth, int virtualHeight)
	{
		SystemManager = new EngineSystemManager();
		EntityManager = new EntityManager(new BasicEntityCollection());
		SystemManager.Add(EntityManager);

		Cam = Entity.Create<Entity>()
			.AddComponent<Camera>();

		Cam.SetViewport(new VirtualViewportScaling(virtualWidth, virtualHeight));

		_target = new RenderTarget2D(Graphics.GraphicsDevice, Cam.Viewport.Width, Cam.Viewport.Height,
			false, SurfaceFormat.Color, DepthFormat.Depth24);

		EnterTime = 0f;
		ExitTime = 0f;
	}

	public virtual void LoadContent()
	{
	}

	public virtual void Init()
	{
		SetState(SceneStates.Entering);
	}

	/// <summary>
	/// before the first frame
	/// </summary>
	public virtual void Start()
	{
		InitedFirstFrame = true;
	}

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
		Graphics.GraphicsDevice.SetRenderTarget(_target);
		Graphics.GraphicsDevice.Clear(new Color(25, 25, 25));

		Graphics.SpriteBatch.Begin(SortMode, BlendState, SamplerState, Stencil, Rasterizer, Effect, Cam.DrawMatrix);
		SystemManager.Draw();
		Graphics.SpriteBatch.End();

		float lerp = State == SceneStates.Active ? 0f :
			State == SceneStates.Entering ? EnterTime.LerpReverse :
			ExitTime.Lerp;

		//  screen enter/exit fade in/out
		if (State != SceneStates.Active)
		{
			Graphics.SpriteBatch.Begin();
			Graphics.SpriteBatch.Draw(Engine.AssetManager.Pixel, new Rectangle(0, 0, Cam.Viewport.Width, Cam.Viewport.Height),
				Engine.AssetManager.Pixel, Color.Black * lerp);
			Graphics.SpriteBatch.End();
		}

		Graphics.GraphicsDevice.SetRenderTarget(null);
		Graphics.GraphicsDevice.Clear(new Color(25, 25, 25));

		Graphics.SpriteBatch.Begin();
		Graphics.SpriteBatch.Draw(_target, Screen.Bounds, Color.White);
		Graphics.SpriteBatch.End();
	}

	public virtual void Exit()
	{
		SetState(SceneStates.Exiting);
	}
}