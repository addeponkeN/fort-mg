using Fort.MG.Systems;
using Fort.Utility;

namespace Fort.MG.EntitySystem;

public class EntityManager : EngineSystem, IRenderable
{
	private readonly IdPool _idPool = new();

	public EntityCollection Collection;

	internal EntityManager(EntityCollection collection)
	{
		SetEntityCollection(collection);
	}

	public void Add(Entity ent)
	{
		ent.Id = _idPool.Get();
		Collection.Add(ent);
	}

	public void Remove(Entity ent)
	{
		Collection.Remove(ent);
		_idPool.Return(ent.Id);
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