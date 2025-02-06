using Fort.MG.Utils;

namespace Fort.MG.EntitySystem;

public abstract class EntityCollection
{
    public abstract void Add(Entity ent);
    public virtual void Remove(Entity ent)
    {
        //PoolManager<Entity>.Free(ent);
    }
    public abstract void Update(IGameTime t);
    public abstract void Render();
    public abstract void Draw();
}