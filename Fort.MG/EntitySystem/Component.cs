using Fort.MG.Utils;

namespace Fort.MG.EntitySystem;

public class Component : BaseEntity
{
	public Entity Entity { get; set; }
	public Transform Transform => Entity.Transform;

	public virtual void OnAdded()
	{
	}

	public virtual void OnDestroyed()
	{
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);
	}

	public static T Create<T>() where T : Component, new()
	{
		var t = new T(); // PoolManager<T>.Spawn();
		t.Init();
		return t;
	}

	public virtual Component AddComponent<T>() where T : Component, new()
	{
		return Entity.AddComponent<T>();
	}
}
