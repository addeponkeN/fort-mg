using Fort.MG.Gui.Components;

namespace Fort.MG.Gui;

internal static class ExtensionsInternal
{
	internal static bool IsActive(this ComponentBase c) => c.IsVisible;
}