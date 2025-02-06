using Fort.MG.EntitySystem;
using Fort.MG.Scenes;
using Fort.MG.Utils;

namespace Fort.MG.Systems;

public abstract class EngineSystem : BaseObject
{
    public EngineSystemManager SystemManager;

    public virtual void OnSceneChanged(Scene scene)
    {
    }

    public virtual void PostUpdate(IGameTime t)
    {
    }
}