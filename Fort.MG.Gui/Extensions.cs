using Fort.MG.Gui.Components;

namespace Fort.MG.Gui
{
	internal static class Extensions
	{
		public static bool IsActive(this ComponentBase c) => c.IsVisible;
	}
}
