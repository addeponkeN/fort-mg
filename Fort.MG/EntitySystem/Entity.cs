using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG.EntitySystem;

public class BaseEntity
{
	internal bool _killed;
	internal bool _initedFirstFrame;
	internal bool _inited;

	public bool Enabled = true;
	public bool IsDestroyed;

	public virtual void Init() { _inited = true; }

	public virtual void Kill()
	{
		_killed = true;
	}

	public virtual void Update(IGameTime t) { }

	public virtual void Render()
	{
	}

	public virtual void Draw()
	{
	}

}

public class Entity : BaseEntity
{
	public Entity Parent;
	public List<Component> Components;
	public Transform Transform;

	public Entity()
	{
		Components = new List<Component>();
	}

	public virtual T Get<T>() where T : Component
	{
		for (int i = 0; i < Components.Count; i++)
		{
			var comp = Components[i];
			if (comp is T) return comp as T;
		}

		return null;
	}

	public virtual T AddComponent<T>() where T : Component, new()
	{
		var comp = Component.Create<T>();
		comp.Entity = this;
		if (!comp._inited)
		{
			comp.Init();
			comp._inited = true;
		}

		Components.Add(comp);
		comp.OnAdded();
		return comp;
	}

	public virtual void AddComponent(Component comp)
	{
		comp.Entity = this;
		if (!comp._inited)
		{
			comp.Init();
			comp._inited = true;
		}

		Components.Add(comp);
		comp.OnAdded();
	}

	public virtual void RemoveComponent(Component comp)
	{
		if (!comp._killed)
			comp.Kill();
		comp.OnDestroyed();
		Components.Remove(comp);
	}

	public virtual void ClearComponents()
	{
		for (int i = 0; i < Components.Count; i++)
			Components[i].OnDestroyed();
		Components.Clear();
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
	public override void Kill()
	{
		base.Kill();
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
			Components[i].OnDestroyed();
		Components.Clear();
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);

		if (!_initedFirstFrame)
		{
			Start();
			_initedFirstFrame = true;
		}

		Transform?.Update(t);
		for (int i = 0; i < Components.Count; i++)
		{
			var c = Components[i];
			if (c._killed)
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
}