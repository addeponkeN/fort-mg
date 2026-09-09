using Fort.MG.Scenes;

namespace Fort.MG.EntitySystem;

public abstract class EntityCollection
{
    /// <summary>
    /// The scene that owns this collection, set once by <see cref="Scene.Init"/>. Lets
    /// implementations (e.g. <see cref="BasicEntityCollection"/>) reach their own scene instead
    /// of the global <see cref="Scene.Current"/>.
    /// </summary>
    public Scene? OwningScene { get; internal set; }

    public abstract void Add(Entity ent);
    public virtual void Remove(Entity ent)
    {
        //PoolManager<Entity>.Free(ent);
    }
    public abstract void Update(IGameContext t);

    public abstract T? GetComponent<T>() where T : Component;
    public abstract Entity? Get(string name);

    public abstract List<IFortRenderable> GetRenderables();

    public abstract void AddRenderable(IFortRenderable renderable);
    public abstract void RemoveRenderable(IFortRenderable renderable);

    public abstract void DrawGizmos();
}