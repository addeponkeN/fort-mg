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
		comp.Canvas = Canvas;
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

	protected void SetComponent<T>(ref T component, T newValue) where T : ComponentBase
	{
		if (component != null)
		{
			RemoveComponent(component);
		}

		component = newValue;

		if (component != null)
		{
			AddComponent(component);
		}

		UpdateTransforms();
	}
}
