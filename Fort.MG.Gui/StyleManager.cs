using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

namespace Fort.MG.Gui;

public class StyleManager
{
	public static Dictionary<Type, Style> Styles = new();
	public static readonly Color BackgroundColor = new Color(35, 35, 35, 255);
	public static readonly Color ForegroundColor = new Color(200, 200, 200, 255);

	public static void Init()
	{
		Styles.Add(typeof(Button), new Style{});
	}

}
