using Fort.MG.EntitySystem;
using Fort.MG.Scenes;

namespace Fort.MG.Systems;

public abstract class EngineSystem : BaseObject
{
    public EngineSystemManager SystemManager { get; internal set; }

    public virtual void PreUpdate(IGameTime gt)
    {

    }

    public virtual void OnSceneChanged(Scene scene)
    {
    }

    public virtual void PostUpdate(IGameTime t)
    {
    }

    public virtual void Render()
    {

    }

    public virtual void Draw()
    {

    }
}