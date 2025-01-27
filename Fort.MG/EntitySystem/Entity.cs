using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem;

public class Entity
{
    public Entity Parent;
    public List<Entity> Components;
    public Transform Transform;

    public bool Enabled = true;
    public bool IsDestroyed;

    internal bool _killed;
    internal bool _initedFirstFrame;
    internal bool _inited;

    public Entity()
    {
        Components = new List<Entity>();
    }

    public virtual T Get<T>() where T : Entity
    {
        for(int i = 0; i < Components.Count; i++)
        {
            var comp = Components[i];
            if(comp is T) return comp as T;
        }

        return null;
    }

    public virtual void AddComponent(Entity comp)
    {
        comp.Parent = this;
        comp.OnAdded();
        if(!comp._inited)
        {
            comp.Init();
            comp._inited = true;
        }

        Components.Add(comp);
    }

    public virtual void RemoveComponent(Entity comp)
    {
        if(!comp._killed)
            comp.Kill();
        Components.Remove(comp);
    }

    public virtual void ClearComponents()
    {
        for(int i = 0; i < Components.Count; i++)
            Components[i].OnDestroyed();
        Components.Clear();
    }

    /// <summary>
    /// initialize
    /// </summary>
    public virtual void Init()
    {
        _inited = true;
    }

    /// <summary>
    /// on first frame
    /// </summary>
    public virtual void Start()
    {
        _initedFirstFrame = true;
    }

    /// <summary>
    /// on added to parent
    /// </summary>
    public virtual void OnAdded()
    {
    }

    /// <summary>
    /// kill this entity
    /// </summary>
    public void Kill()
    {
        _killed = true;
    }

    /// <summary>
    /// when it gets removed from the world
    /// </summary>
    public virtual void OnDestroyed()
    {
        if(IsDestroyed)
        {
            //Log.W($"entity tried to destroy more than once - {GetType().Name} ");
            return;
        }

        IsDestroyed = true;
        for(int i = 0; i < Components.Count; i++)
            Components[i].OnDestroyed();
        Components.Clear();
    }

    public virtual void Update(IGameTime t)
    {
        if(!_initedFirstFrame)
        {
            Start();
            _initedFirstFrame = true;
        }

        Transform?.Update(t);
        for(int i = 0; i < Components.Count; i++)
        {
            var c = Components[i];
            if(c._killed)
            {
                c.OnDestroyed();
                RemoveComponent(c);
                i--;
                // Components.RemoveAt(i--);
                continue;
            }

            if(c.Enabled)
                c.Update(t);
        }
    }

    public virtual void Render()
    {
        for(int i = 0; i < Components.Count; i++)
        {
            var c = Components[i];
            if(!c.Enabled)
                continue;
            c.Render();
        }
    }

    public virtual void Draw()
    {
        for(int i = 0; i < Components.Count; i++)
        {
            var c = Components[i];
            if(!c.Enabled)
                continue;
            c.Draw();
        }
    }

    /// <summary>
    /// creates and returns a new entity 
    /// </summary>
    public static T Create<T>() where T : Entity, new()
    {
        var t = new T(); // PoolManager<T>.Spawn();
        t.Spawn();
        t.Init();
        return t;
    }

    /// <summary>
    /// creates a new entity and adds it 
    /// </summary>
    public static T Instantiate<T>() where T : Entity, new()
    {
        var ent = Create<T>();
        Engine.SceneManager.Scene.EntityManager.Add(ent);
        return ent;
    }

    /// <summary>
    /// adds an entity
    /// </summary>
    public static T Instantiate<T>(T existingEnt) where T : Entity, new()
    {
        Engine.SceneManager.Scene.EntityManager.Add(existingEnt);
        return existingEnt;
    }

    public virtual void Spawn()
    {
        Parent = null;
        _killed = false;
        IsDestroyed = false;
        _initedFirstFrame = false;
        // UpdateLife = true;
        // Life = new AgoLife();
        Transform = null;
        Enabled = true;

        SetupTransform();
    }

    protected void SetupTransform()
    {
        if(!(this is Transform))
        {
            Transform = Create<Transform>();
            Transform.Parent = this;
            Transform.Init();
            Transform.Position = Vector2.Zero;
            Transform.Size = Vector2.Zero;
            Transform.Z = 0f;
            Transform.LocalPosition = Vector2.Zero;
            Transform.LocalZ = 0f;
        }
    }

    public virtual void Free()
    {
        Parent = null;
        Transform = null;
    }
}