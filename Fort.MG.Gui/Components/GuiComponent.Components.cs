namespace Fort.MG.Gui.Components;

public partial class GuiComponent
{
	protected List<ComponentBase> Components { get; } = new();

	public void AddComponent(ComponentBase comp)
	{
		if (comp == null)
			return;

		Components.Add(comp);
		comp.Parent = this;
	}

	public bool RemoveComponent(ComponentBase comp)
	{
		if (comp == null)
		{
			return false;
		}

		comp.Parent = null;

		return Components.Remove(comp);
	}

	protected void SetComponent<T>(ref T field, T value) where T : ComponentBase
	{
		if (field != null)
		{
			RemoveComponent(field);
		}

		field = value;
		field.Canvas = Canvas;

		if (field != null)
		{
			AddComponent(field);
		}

		UpdateTransforms();
	}
}
