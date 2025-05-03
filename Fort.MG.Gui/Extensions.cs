using Fort.MG.Gui.Components;

namespace Fort.MG.Gui;

public static class Extensions
{
	public static Window? GetParentWindow(this ComponentBase? component)
	{
		while (true)
		{
			if (component == null) return null;
			if (component is Window window)
				return window;
			component = component.Parent;
		}
	}
}