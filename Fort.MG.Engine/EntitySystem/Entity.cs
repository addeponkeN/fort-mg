using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem;

public class BaseObject
{
	internal bool InitedFirstFrame = false;
	internal bool Inited;

	public bool Enabled { get; set; } = true;
	public bool IsDestroyed { get; internal set; }

	/// <summary>
	/// One time init - called when the entity object is created - once ever.
	/// </summary>
	public virtual void Init() { Inited = true; }

    /// <summary>
    /// Called on the first frame after Init
    /// </summary>
    public virtual void Start()
	{
		InitedFirstFrame = true;
	}

	/// <summary>
	/// Marks the object as destroyed, preventing further operations that require it to be active
	/// </summary>
	public virtual void Destroy()
	{
        IsDestroyed = true;
	}

	internal virtual void UpdateFirstFrame(IGameTime t)
	{
		if (!InitedFirstFrame)
		{
			Start();
			InitedFirstFrame = true;
		}
	}

	public virtual void Update(IGameTime t)
	{
		if (!InitedFirstFrame)
		{
			Start();
			InitedFirstFrame = true;
		}
	}

	public virtual void Render()
	{
	}

	public virtual void Draw()
	{
	}
}


public class Entity : BaseObject
{
    internal EntityCollection _collection;

	public int Id { get; set; }
	public string Name { get; set; }

	public Entity Parent;
	public List<Component> Components;
	public Transform Transform;

	public Entity()
	{
		Components = new List<Component>();
	}

	public virtual T GetComponent<T>() where T : Component
	{
		for (int i = 0; i < Components.Count; i++)
		{
			var comp = Components[i];
			if (comp is T retComp) return retComp;
		}

		return null!;
	}

	public virtual T AddComponent<T>() where T : Component, new()
	{
		var comp = Component.Create<T>();
		comp.Entity = this;

        if (comp is IFortRenderable renderable)
        {
			_collection?.AddRenderable(renderable);
        }

		if (!comp.Inited)
		{
			comp.Init();
			comp.Inited = true;
		}

		Components.Add(comp);
		comp.OnAdded();
		return comp;
	}

	public virtual void AddComponent(Component comp)
	{
		comp.Entity = this;

        if (comp is IFortRenderable renderable)
        {
            _collection?.AddRenderable(renderable);
        }

        if (!comp.Inited)
		{
			comp.Init();
			comp.Inited = true;
		}

		Components.Add(comp);
		comp.OnAdded();
	}

	public virtual void RemoveComponent(Component comp)
	{
		if (!comp.IsDestroyed)
			comp.Destroy();
		comp.OnDestroyed();

        if (comp is IFortRenderable renderable)
        {
            _collection.RemoveRenderable(renderable);
        }

        Components.Remove(comp);
	}

	/// <summary>
	/// initialize
	/// </summary>
	public override void Init()
	{
		base.Init();
	}

	/// <summary>
	/// on first frame
	/// </summary>
	public override void Start()
	{
		base.Start();
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
	public override void Destroy()
	{
		base.Destroy();
	}

	/// <summary>
	/// when it gets removed from the world
	/// </summary>
	public virtual void OnDestroyed()
	{
		if (IsDestroyed)
		{
			//Log.W($"entity tried to destroy more than once - {GetType().Name} ");
			return;
		}

		IsDestroyed = true;
        for (int i = 0; i < Components.Count; i++)
        {
			var comp = Components[i];
            comp.OnDestroyed();
            if (comp is IFortRenderable renderable)
            {
                _collection.RemoveRenderable(renderable);
            }
        }
		Components.Clear();
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);

		Transform?.Update(t);

		for (int i = 0; i < Components.Count; i++)
		{
			var c = Components[i];
			if (!c.InitedFirstFrame)
				c.UpdateFirstFrame(t);
		}

		for (int i = 0; i < Components.Count; i++)
		{
			var c = Components[i];
			if (c.IsDestroyed)
			{
				RemoveComponent(c);
				i--;
				continue;
			}

			if (c.Enabled)
				c.Update(t);
		}
	}

	public override void Render()
	{
		for (int i = 0; i < Components.Count; i++)
		{
			var c = Components[i];
			if (!c.Enabled)
				continue;
			c.Render();
		}
	}

	public override void Draw()
	{
		for (int i = 0; i < Components.Count; i++)
		{
			var c = Components[i];
			if (!c.Enabled)
				continue;
			c.Draw();
		}
	}

	/// <summary>
	/// Create an Entity with a component attached
	/// </summary>
	public static T Create<T>() where T : Component, new()
	{
		var ent = Create();
		return ent.AddComponent<T>();
	}

	/// <summary>
	/// Instantiate an entity with a component attached
	/// </summary>
	public static T Instantiate<T>() where T : Component, new()
	{
		var ent = Instantiate();
		return ent.AddComponent<T>();
	}

	/// <summary>
	/// Create an entity
	/// </summary>
	public static Entity Create()
	{
		var t = new Entity(); // PoolManager<T>.Spawn();
		t.Spawn();
		t.Init();
		return t;
	}

	/// <summary>
	/// Create and instantiate an entity
	/// </summary>
	public static Entity Instantiate()
	{
		var ent = Create();
		return Instantiate(ent);
	}

	/// <summary>
	/// Instantiate an existing entity
	/// </summary>
	public static Entity Instantiate(Entity existingEnt)
	{
		FortEngine.SceneManager.Scene.EntityManagerSystem.Add(existingEnt);
		return existingEnt;
	}

	internal virtual void Spawn()
	{
		Parent = null;
		IsDestroyed = false;
		InitedFirstFrame = false;
		// UpdateLife = true;
		// Life = new AgoLife();
		Transform = null;
		Enabled = true;

		SetupTransform();
	}

	protected void SetupTransform()
	{
		Transform = Component.Create<Transform>();
		Transform.Entity = this;
		Transform.Init();
		Transform.Position3 = Vector3.Zero;
		Transform.LocalPosition3 = Vector3.Zero;
		Transform.Size = Vector2.Zero;
		Transform.CalculateWorldPosition();
		AddComponent(Transform);
	}

	public virtual void Free()
	{
		Parent = null;
		Transform = null;
	}

	public override string ToString()
	{
		var firstCompName = Components.Count > 1 ? Components[1].GetType().Name : "transform";
		return $"({Id}){Name}-{firstCompName} ({Transform.Position})";
	}

}