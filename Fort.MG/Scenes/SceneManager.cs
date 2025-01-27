using Fort.MG.EntitySystem;
using Fort.MG.Systems;
using Fort.MG.Utils;

namespace Fort.MG.Scenes;

//  aka scene aka world
public class SceneManager : EngineSystem, IRenderable
{
    Type _defaultSceneType;

    Scene DefaultScene
    {
        //  todo - very slow
        get => Activator.CreateInstance(_defaultSceneType) as Scene;
    }

    Queue<Scene> queuedScenes;
    List<Scene> popupScenes;

    public Scene Scene;

    public SceneManager()
    {
        queuedScenes = new Queue<Scene>();
        popupScenes = new List<Scene>();
    }

    public void SetScene(Scene scene)
    {
        //  automatically set default scene to first set scene
        _defaultSceneType ??= scene.GetType();

        if(Scene == null)
        {
            SetActiveScene(scene);
        }
        else
        {
            Scene.Exit();
            queuedScenes.Enqueue(scene);
        }
    }

    void SetActiveScene(Scene scene)
    {
        Scene = scene;
        Scene.State = SceneStates.Entering;
        Scene.Init();
        Scene.LoadContent();
        Engine.SystemManager.OnSceneChanged(scene);
    }

    public override void Update(IGameTime t)
    {
        if(Scene != null)
        {
            Scene.Update(t);
            if(Scene.State == SceneStates.Exited)
            {
                SetActiveScene(queuedScenes.Count > 0 ? queuedScenes.Dequeue() : DefaultScene);
            }
        }
        else
        {
            SetActiveScene(queuedScenes.Count > 0 ? queuedScenes.Dequeue() : DefaultScene);
        }

        base.Update(t);
    }

    public override void Render()
    {
        base.Render();
        if(Scene != null)
            Scene.Render();
    }

    public override void Draw()
    {
        base.Draw();
        if(Scene != null)
            Scene.Draw();
    }
}