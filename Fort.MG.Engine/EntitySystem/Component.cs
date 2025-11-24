namespace Fort.MG.EntitySystem;

public abstract class Component : BaseObject
{
    private static int _idPool = 1;
	public int Id { get; set; }
    public string Name { get; set; }
	public Entity Entity { get; set; }

	public Transform Transform => Entity?.Transform;

    protected Component()
    {
        Id = _idPool++;
    }

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

    public virtual T GetComponent<T>(string name) where T : Component
    {
        return Entity.GetComponent<T>(name);
    }

    // Serialization methods
    internal virtual void OnBeforeSerialize() { }
	internal virtual void OnAfterDeserialize() { }
}
