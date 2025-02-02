namespace Fort.MG.EntitySystem;

public class Component : BaseEntity
{
	public int Id { get; set; }
	public Entity Entity { get; set; }
	public Transform Transform => Entity.Transform;

	public virtual void OnAdded()
	{
	}

	public virtual void OnDestroyed()
	{
	}

	public static T Create<T>() where T : Component, new()
	{
		var t = new T(); // PoolManager<T>.Spawn();
		t.Init();
		return t;
	}

	public virtual T AddComponent<T>() where T : Component, new()
	{
		return Entity.AddComponent<T>();
	}

	public virtual T GetComponent<T>() where T : Component
	{
		return Entity.GetComponent<T>();
	}
}
