using Fort.MG.Systems;

namespace Fort.MG.EntitySystem;

public class EntityManager : EngineSystem
{

    public EntityCollection Collection;

    internal EntityManager(EntityCollection collection)
    {
        SetEntityCollection(collection);
    }

    public T? GetComponent<T>() where T : Component => Collection.GetComponent<T>();
    public Entity? GetEntity(string name) => Collection.Get(name);

    public void Add(Entity ent)
    {
        Collection.Add(ent);
    }

    public void Remove(Entity ent)
    {
        Collection.Remove(ent);
    }

    public void SetEntityCollection(EntityCollection collection)
    {
        this.Collection = collection;
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);
        Collection.Update(t);
    }

    public List<IFortRenderable> GetRenderables()
    {
        return Collection.GetRenderables();
    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();
        Collection.DrawGizmos();
    }
}