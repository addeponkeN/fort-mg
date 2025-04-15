using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui;

public class StyleManager
{
	public static Dictionary<Type, Style> Styles = new();
	public static readonly Color Background1Color = new Color(40, 40, 40, 255);
	public static readonly Color Background2Color = new Color(30, 30, 30, 255);
	public static readonly Color Foreground1Color = new Color(210, 210, 210, 255);
	public static readonly Color Foreground2Color = new Color(180, 180, 180, 255);

	public static void Init()
	{
		Styles.Add(typeof(Button), new Style{});
	}

}
