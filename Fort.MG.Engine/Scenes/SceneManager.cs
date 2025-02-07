using Fort.MG.EntitySystem;
using Fort.MG.Systems;

namespace Fort.MG.Scenes;

//  aka scene aka world
public class SceneManager : EngineSystem, IRenderable
{
	public static Scene CurrentScene => FortEngine.SceneManager.Scene;

	private Type? _defaultSceneType;

	private Scene CreateDefaultScene => _defaultSceneType == null ? new Scene() : Activator.CreateInstance(_defaultSceneType) as Scene;

	private Queue<Scene> _queuedScenes;
	private List<Scene> _popupScenes;

	public Scene Scene { get; private set; }

	public SceneManager()
	{
		_queuedScenes = new Queue<Scene>();
		_popupScenes = new List<Scene>();
	}

	public void SetScene(Scene scene)
	{
		scene.SceneManager = this;
		//  automatically set default scene to first set scene
		_defaultSceneType ??= scene.GetType();

		if (Scene == null)
		{
			SetActiveScene(scene);
		}
		else
		{
			Scene.Exit();
			_queuedScenes.Enqueue(scene);
		}
	}

	private void SetActiveScene(Scene scene)
	{
		Scene = scene;
		Scene.State = SceneStates.Entering;
		if(!Scene.IsInited)
			Scene.Init();
		Scene.IsInited = true;
		if (!Scene.IsLoaded)
			Scene.LoadContent();
		Scene.IsLoaded = true;
		FortEngine.SystemManager.OnSceneChanged(scene);
	}

	public override void Update(IGameTime t)
	{
		if (Scene != null)
		{
			Scene.Update(t);
			if (Scene.State == SceneStates.Exited)
			{
				SetActiveScene(_queuedScenes.Count > 0 ? _queuedScenes.Dequeue() : CreateDefaultScene);
			}
		}
		else
		{
			SetActiveScene(_queuedScenes.Count > 0 ? _queuedScenes.Dequeue() : CreateDefaultScene);
		}

		base.Update(t);
	}

	public override void Render()
	{
		base.Render();
		if (Scene != null)
			Scene.Render();
	}

	public override void Draw()
	{
		base.Draw();
		if (Scene != null)
			Scene.Draw();
	}
}