using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG.Utils;

public static class Graphics
{
	public static SpriteBatch SpriteBatch { get; private set; }
	public static GraphicsDevice GraphicsDevice { get; private set; }

	internal static void Init()
	{
		GraphicsDevice = Engine.GraphicsDeviceManager.GraphicsDevice;
		SpriteBatch = new SpriteBatch(GraphicsDevice);
	}
}