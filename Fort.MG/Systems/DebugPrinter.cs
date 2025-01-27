using System.Diagnostics;
using FontStashSharp;
using Fort.MG.Core;
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
    static Vector2 Position = new Vector2(4, 80);
    static int spacing = 14;
    int i;

    int fps;
    float ms;

    Stopwatch sw;

    public DebugPrinter()
    {
        sw = new Stopwatch();
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);
        fps += (int)((1f / Engine.gt.ElapsedGameTime.TotalSeconds - fps) * 0.1);
    }

    public override void Draw()
    {
        i = 0;
        Draw($"fps: {fps:N0}");
        Draw($"ms: {ms:N5}");
        Draw($"swaps: {Graphics.GraphicsDevice.Metrics.TextureCount}");
        var cam = Engine.Cam;
        Draw($"");
        Draw($"mouse: {Input.MousePos}");
        Draw($"mouse world: {Input.MouseTransformedPos(cam.UpdateMatrix)}");
        Draw($"cam: {cam.Bounds}");
        // var rec = TileHelper.WorldBoundsToChunkBounds(cam.Bounds);
        // Draw($"chunkbounds: x{rec.X}, y{rec.Y}, r{rec.Right}, b{rec.Bottom}");
    }

    private void Draw(string text)
    {
        Graphics.SpriteBatch.DrawString(Engine.Content.DefaultFont, text,
            new Vector2(Position.X + 1, Position.Y + i * spacing + 1),
            Color.Black);
        Graphics.SpriteBatch.DrawString(Engine.Content.DefaultFont, text, new Vector2(Position.X, Position.Y + i++ * spacing),
            Color.White);
    }

    public void OnDrawBegin()
    {
        sw.Restart();
    }

    public void OnDrawEnd()
    {
        sw.Stop();
        ms = (float)sw.Elapsed.TotalMilliseconds;
    }
}