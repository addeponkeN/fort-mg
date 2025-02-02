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
	private readonly List<IRenderable> _renderables = new();
	public List<Entity> Entities = new();

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