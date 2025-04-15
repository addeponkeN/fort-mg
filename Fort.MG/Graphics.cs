using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Fort.MG;

public static class Graphics
{
	private static Game Game => FortCore.Game;

	public static SpriteBatch SpriteBatch { get; private set; }
	public static GraphicsDevice GraphicsDevice { get; private set; }
	public static GraphicsDeviceManager GDM { get; private set; }

	internal static void Start()
	{
		GDM = new GraphicsDeviceManager(Game);
	}

	internal static void Init()
	{
		if (Game == null)
			throw new InvalidOperationException("Game instance is not set. Call Start() first.");

		GraphicsDevice = Game.GraphicsDevice;
		SpriteBatch = new SpriteBatch(GraphicsDevice);
	}

	public static void Dispose()
	{
		SpriteBatch?.Dispose();
		GDM?.Dispose();
	}

	public static Texture2D CopyTextureRegion(Texture2D sourceTexture, Rectangle sourceRegion)
	{
		Color[] data = new Color[sourceRegion.Width * sourceRegion.Height];
		sourceTexture.GetData(0, sourceRegion, data, 0, data.Length);

		Texture2D newTexture = new Texture2D(GraphicsDevice, sourceRegion.Width, sourceRegion.Height);
		newTexture.SetData(data);

		return newTexture;
	}

	public static void SetRenderTarget(RenderTarget2D target)
	{
		GraphicsDevice.SetRenderTarget(target);
	}

	public static void ResetRenderTarget()
	{
		GraphicsDevice.SetRenderTarget(null);
	}

	public static void Clear(Color color)
	{
		GraphicsDevice.Clear(color);
	}
}