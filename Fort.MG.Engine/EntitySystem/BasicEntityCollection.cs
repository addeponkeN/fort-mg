using Fort.MG.Rendering;
using Fort.MG.Scenes;
using Fort.MG.Systems;
using Fort.Utility;

namespace Fort.MG.EntitySystem;

public interface IFortRenderable
{
    void Draw();
    void Render();
    bool Enabled { get; }
    float DrawLayer { get; }
    bool IsDestroyed { get; }
    RenderPass RenderPass { get; }
}

public interface IFortDrawableGui
{
    void DrawGui();
    bool Enabled { get; }
}

public interface IFortDrawable
{
    void OnDrawBegin();
    void OnDrawEnd();
    bool Enabled { get; }
}

public class BasicEntityCollection : EntityCollection
{
    private readonly IdPool _idPool = new();

    private SystemMessageSystem _messageSystem;

    private readonly List<IFortRenderable> _renderables = new();
    private readonly List<Entity> _newEntities = new();
    private readonly List<Entity> _initedEntities = new();

    public List<Entity> Entities = new();

    public BasicEntityCollection()
    {
        _messageSystem = FortEngine.GetSystem<SystemMessageSystem>();
    }

    public override void Add(Entity ent)
    {
        ent._collection = this;
        ent.Id = _idPool.Get();
        _newEntities.Add(ent);
        Entities.Add(ent);
        foreach (IFortRenderable renderable in ent.Components.Where(x => x is IFortRenderable))
        {
            AddRenderable(renderable);
        }
    }

    public override void Remove(Entity ent)
    {
        _idPool.Return(ent.Id);
        ent.OnDestroyed();
        _initedEntities.Remove(ent);
        Entities.Remove(ent);
        foreach (IFortRenderable renderable in ent.Components.Where(x => x is IFortRenderable))
        {
            RemoveRenderable(renderable);
        }
        ent._collection = null;
    }

    public override void Update(IGameTime t)
    {
        for (int i = 0; i < _newEntities.Count; i++)
        {
            var e = _newEntities[i];
            e.UpdateFirstFrame(t);
            _newEntities.RemoveAt(i--);
            _initedEntities.Add(e);
        }

        for (var i = 0; i < _renderables.Count; i++)
        {
            var renderable = _renderables[i];
            if (renderable.IsDestroyed)
            {
                _renderables.RemoveAt(i--);
            }
        }

        for (int i = 0; i < _initedEntities.Count; i++)
        {
            var e = _initedEntities[i];
            if (e.InitedFirstFrame)
            {
                try
                {
                    e.Update(t);
                }
                catch (Exception exception)
                {
                    _messageSystem.AddMessage($"Error in entity update: {e.Name}: {exception.Message}", true);
                }
            }
        }

        for (int i = 0; i < _initedEntities.Count; i++)
        {
            var e = _initedEntities[i];
            if (e.IsDestroyed)
            {
                Remove(e);
                i--;
            }
        }

    }

    public override void Render()
    {
        //for (int i = 0; i < _initedEntities.Count; i++)
        //{
        //    var e = _initedEntities[i];
        //    if (e.InitedFirstFrame)
        //        e.Render();
        //}
    }

    public override void Draw()
    {
        //for (int i = 0; i < _initedEntities.Count; i++)
        //{
        //    var e = _initedEntities[i];
        //    if (e.InitedFirstFrame)
        //        e.Draw();
        //}
    }

    public override T GetComponent<T>()
    {
        foreach (var ent in Entities)
        {
            foreach (var comp in ent.Components)
            {
                if (comp is T tComp)
                    return tComp;
            }
        }

        return null!;
    }

    public override Entity? Get(string name)
    {
        return Entities.FirstOrDefault(x => x.Name == name);
    }

    public override List<IFortRenderable> GetRenderables()
    {
        return _renderables;
    }

    public override void AddRenderable(IFortRenderable renderable)
    {
        _renderables.Add(renderable);
        Scene.Current._renderPassManager.MarkDirty();
    }

    public override void RemoveRenderable(IFortRenderable renderable)
    {
        _renderables.Remove(renderable);
        Scene.Current._renderPassManager.MarkDirty();
    }
}