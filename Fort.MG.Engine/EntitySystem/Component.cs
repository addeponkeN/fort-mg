namespace Fort.MG.EntitySystem;

public abstract class Component : BaseObject
{
	public int Id { get; set; }
	public Entity Entity { get; set; }

	public Transform Transform => Entity?.Transform;

	public virtual void OnAdded() { }
	public virtual void OnDestroyed() { }

	public static T Create<T>() where T : Component, new()
	{
		var t = new T();
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

	// Serialization methods
	internal virtual void OnBeforeSerialize() { }
	internal virtual void OnAfterDeserialize() { }
}
