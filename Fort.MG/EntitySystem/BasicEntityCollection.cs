using Fort.MG.Utils;

namespace Fort.MG.EntitySystem;

public interface IRenderable
{
	void Draw();
	void Render();
}

public interface IDrawableGui
{
	void DrawGui();
}

public interface IDrawableControl
{
	void OnDrawBegin();
	void OnDrawEnd();
}

public class BasicEntityCollection : EntityCollection
{
	public List<Entity> Entities;
	List<IRenderable> _renderables;

	public BasicEntityCollection()
	{
		Entities = new List<Entity>();
		_renderables = new List<IRenderable>();
	}

	public override void Add(Entity ent)
	{
		// ent.Start();
		Entities.Add(ent);
		if (ent is IRenderable entRenderable)
			_renderables.Add(entRenderable);
	}

	public override void Remove(Entity ent)
	{
		ent.OnDestroyed();
		Entities.Remove(ent);
		if (ent is IRenderable entRenderable)
			_renderables.Remove(entRenderable);
	}

	public override void Update(IGameTime t)
	{
		for (int i = 0; i < Entities.Count; i++)
		{
			var e = Entities[i];
			if (e._killed)
			{
				Remove(e);
				continue;
			}

			e.Update(t);
		}
	}

	public override void Render()
	{
		for (int i = 0; i < Entities.Count; i++)
		{
			Entities[i].Render();
		}
	}

	public override void Draw()
	{
		for (int i = 0; i < Entities.Count; i++)
		{
			Entities[i].Draw();
		}
	}
}