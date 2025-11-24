namespace Fort.MG.EntitySystem;

public abstract class EntityCollection
{
    public abstract void Add(Entity ent);
    public virtual void Remove(Entity ent)
    {
        //PoolManager<Entity>.Free(ent);
    }
    public abstract void Update(IGameTime t);

    public abstract T? GetComponent<T>() where T : Component;
    public abstract Entity? Get(string name);

    public abstract List<IFortRenderable> GetRenderables();

    public abstract void AddRenderable(IFortRenderable renderable);
    public abstract void RemoveRenderable(IFortRenderable renderable);

    public abstract void DrawGizmos();
}