using Fort.MG.Systems;
using Fort.MG.Utils;

namespace Fort.MG.EntitySystem;

public class EntityManager : EngineSystem, IRenderable
{
    public EntityCollection Collection;

    internal EntityManager(EntityCollection collection)
    {
        SetEntityCollection(collection);
    }

    public void Add(Entity ent)
    {
        Collection.Add(ent);
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

    public override void Render()
    {
        // base.Render();
        Collection.Render();
    }

    public override void Draw()
    {
        // base.Draw();
        Collection.Draw();
    }
}