using System.Diagnostics;
using Fort.MG.Gui.Components;
using Microsoft.Xna.Framework;

namespace Fort.MG.Systems;

public interface IDebugDrawable
{
	void DrawDebug();
}

public class DebugPrinter : EngineSystem
{
	private static Vector2 _position = new Vector2(4, 10);
	private static int _spacing = 14;
	private int _i;

	private int _fps;
	private float _ms;

	private Stopwatch _sw;

	private TextRenderer _textRenderer;

	public DebugPrinter()
	{
		_sw = new Stopwatch();
	}

	public override void Start()
	{
		base.Start();
		_textRenderer = new();
	}

	public override void Update(IGameTime t)
	{
		base.Update(t);
		_fps += (int)((1f / FortEngine.Time.ElapsedGameTime.TotalSeconds - _fps) * 0.1);
	}

	public override void Draw()
	{
		_i = 0;
		var cam = FortEngine.Cam;
		var mouse = Input.MousePos;
		var mouseWorld = Input.MouseTransformedPos(cam.UpdateMatrix);

		Draw($"fps: {_fps:N0}");
		Draw($"ms: {_ms:N5}");
		Draw($"swaps: {Graphics.GraphicsDevice.Metrics.TextureCount}");
		Draw($"");
		Draw($"mouse: {mouse.X}, {mouse.Y}");
		Draw($"mouse world: {mouseWorld.X}, {mouseWorld.Y}");
		Draw($"cam: {cam.Bounds}");
	}

	private void Draw(string text)
	{
		_textRenderer.Position = new Vector2(_position.X, _position.Y + _i++ * _spacing);
		_textRenderer.Text = text;
		_textRenderer.DrawText();
	}

	public void OnDrawBegin()
	{
		_sw.Restart();
	}

	public void OnDrawEnd()
	{
		_sw.Stop();
		_ms = (float)_sw.Elapsed.TotalMilliseconds;
	}
}