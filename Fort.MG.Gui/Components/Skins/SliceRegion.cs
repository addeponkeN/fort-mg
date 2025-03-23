using Microsoft.Xna.Framework;

namespace Fort.MG.Gui.Components;

public struct SliceRegion
{
	public Rectangle Source { get; set; }
	public Rectangle Destination { get; set; }

	public SliceRegion(Rectangle source, Rectangle destination)
	{
		Source = source;
		Destination = destination;
	}
}
