using Fort.MG.EntitySystem;
using Fort.MG.Gui.Components;
using Fort.MG.Utils;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Fort.MG.Systems;

internal class PerformanceMetricsSystem : EngineSystem, IFortDrawableGui, IFortDrawable
{
    public Vector2 Position { get; set; } = new Vector2(4);
    private TextRenderer _textRenderer;

    private double _frameTime;
    private double _updateTime;
    private double _drawTime;
    private double _fps;
    private double _lastCompleteFrameTime;
    private double _frameStart;
    private double _updateStart;
    private double _drawStart;

    private readonly Queue<double> _frameTimeSamples = new();
    private readonly Queue<double> _updateTimeSamples = new();
    private readonly Queue<double> _drawTimeSamples = new();
    private const int MaxSamples = 60;

    private float _lineSpacing = 4f;
    private FortTimer _updateTimer;

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public override void Start()
    {
        base.Start();
        _textRenderer = new();
        _textRenderer.Shadow = true;

        _lastCompleteFrameTime = GetPreciseTimestamp();

        _updateTimer = new(1f);
        _updateTimer.OnTick += () =>
        {
            long managedBytes = GC.GetTotalMemory(false) / 1000000;
            long totalBytes = Process.GetCurrentProcess().WorkingSet64 / 1000000;
            FortCore.Game.Window.Title =
                $"FortEngine - " +
                $"[{Screen.Width}x{Screen.Height}] " +
                $"[FPS: {_fps:0}] " +
                $"[FT: {_frameTime:0.00}ms] " +
                $"[UT: {_updateTime:0.00}ms] " +
                $"[DT: {_drawTime:0.00}ms] " +
                $"[Managed: {managedBytes} MB] " +
                $"[Total: {totalBytes} MB]";
        };
        _updateTimer.Start();
        _updateTimer.Loop = true;
    }

    public override void PreUpdate(IGameTime gt)
    {
        base.PreUpdate(gt);

        _frameStart = GetPreciseTimestamp();
    }

    public override void Update(IGameTime t)
    {
        base.Update(t);

        _updateStart = GetPreciseTimestamp();
    }

    public override void PostUpdate(IGameTime t)
    {
        base.PostUpdate(t);

        double currentTime = GetPreciseTimestamp();
        double updateTimeMs = currentTime - _updateStart;

        _updateTimeSamples.Enqueue(updateTimeMs);
        if (_updateTimeSamples.Count > MaxSamples)
            _updateTimeSamples.Dequeue();

        _updateTime = _updateTimeSamples.Average();

        _frameTime = currentTime - _frameStart;
    }

    public void DrawGui()
    {
        Vector2 drawPos = Position;
        DrawMetric($"FPS: {_fps:0.0}", ref drawPos);
        DrawMetric($"Frame: {_frameTime:0.00} ms", ref drawPos);
        DrawMetric($"Update: {_updateTime:0.00} ms", ref drawPos);
        DrawMetric($"Draw: {_drawTime:0.00} ms", ref drawPos);
    }

    public void OnDrawBegin()
    {
        _drawStart = GetPreciseTimestamp();
    }

    public void OnDrawEnd()
    {
        double drawEnd = GetPreciseTimestamp();
        double currentDrawTime = drawEnd - _drawStart;

        _drawTimeSamples.Enqueue(currentDrawTime);
        if (_drawTimeSamples.Count > MaxSamples)
            _drawTimeSamples.Dequeue();

        _drawTime = _drawTimeSamples.Average();

        double actualFrameTime = drawEnd - _lastCompleteFrameTime;
        _lastCompleteFrameTime = drawEnd;

        _frameTimeSamples.Enqueue(actualFrameTime);
        if (_frameTimeSamples.Count > MaxSamples)
            _frameTimeSamples.Dequeue();

        double avgFrameTime = _frameTimeSamples.Average();
        _fps = avgFrameTime > 0 ? 1000.0 / avgFrameTime : 0.0;
    }

    private void DrawMetric(string text, ref Vector2 position)
    {
        _textRenderer.Text = text;
        _textRenderer.Position = position;
        _textRenderer.DrawText();
        position.Y += 12 + _lineSpacing;
    }

    private double GetPreciseTimestamp()
    {
        return _stopwatch.Elapsed.TotalMilliseconds;
    }

    private static string FormatBytes(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024f:0.0} KB";
        return $"{bytes / (1024f * 1024f):0.0} MB";
    }
}