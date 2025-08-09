using Microsoft.Xna.Framework;

namespace Fort.MG.VirtualViewports;

public class VirtualViewport
{
	public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

	public Vector2 Scale => new Vector2((float)Screen.Width / Width, (float)Screen.Height / Height);

	public int Width;
	public int Height;

	public virtual Matrix Matrix => Matrix.Identity;

	public VirtualViewport() : this(Screen.Width, Screen.Height)
	{
	}

	protected VirtualViewport(int width, int height)
	{
		this.Width = width;
		this.Height = height;
	}
}