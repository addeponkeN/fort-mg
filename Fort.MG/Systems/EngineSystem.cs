using Fort.MG.EntitySystem;
using Fort.MG.Scenes;
using Fort.MG.Utils;

namespace Fort.MG.Systems;

public abstract class EngineSystem : Entity
{
    public EngineSystemManager SystemManager;
    
    protected EngineSystem() : base()
    {
        SetupTransform();
    }

    public virtual void OnSceneChanged(Scene scene)
    {
        for(int i = 0; i < Components.Count; i++)
        {
            var comp = Components[i];
            if(comp is EngineSystem es)
                es.OnSceneChanged(scene);
        }
    }

    public virtual void PostUpdate(IGameTime t)
    {
        // for(int i = 0; i < SystemManager.Systems.Count; i++)
            // SystemManager.Systems[i].PostUpdate(t);
    }
    
}