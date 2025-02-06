using System.Diagnostics;
using FontStashSharp;
using Fort.MG.EntitySystem;
using Fort.MG.Utils;
using Microsoft.Xna.Framework;

namespace Fort.MG.Systems;

public interface IDebugDrawable
{
    void DrawDebug();
}

public class DebugPrinter : EngineSystem, IRenderable, IDrawableControl
{
	private static Vector2 _position = new Vector2(4, 10);
	private static int _spacing = 14;
	private int _i;

	private int _fps;
	private float _ms;

	private Stopwatch _sw;

    public DebugPrinter()
    {
        _sw = new Stopwatch();
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);
        _fps += (int)((1f / FortEngine.Gt.ElapsedGameTime.TotalSeconds - _fps) * 0.1);
    }

    public override void Draw()
    {
        _i = 0;
        Draw($"fps: {_fps:N0}");
        Draw($"ms: {_ms:N5}");
        Draw($"swaps: {Graphics.GraphicsDevice.Metrics.TextureCount}");
        var cam = FortEngine.Cam;
        Draw($"");
        Draw($"mouse: {Input.MousePos}");
        Draw($"mouse world: {Input.MouseTransformedPos(cam.UpdateMatrix)}");
        Draw($"cam: {cam.Bounds}");
        // var rec = TileHelper.WorldBoundsToChunkBounds(cam.Bounds);
        // Draw($"chunkbounds: x{rec.X}, y{rec.Y}, r{rec.Right}, b{rec.Bottom}");
    }

    private void Draw(string text)
    {
        Graphics.SpriteBatch.DrawString(FortEngine.AssetManager.DefaultFont, text,
            new Vector2(_position.X + 1, _position.Y + _i * _spacing + 1),
            Color.Black);
        Graphics.SpriteBatch.DrawString(FortEngine.AssetManager.DefaultFont, text, new Vector2(_position.X, _position.Y + _i++ * _spacing),
            Color.White);
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