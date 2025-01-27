using Fort.MG.Extensions;

namespace Fort.MG.Utils;

/// <summary>
/// if possible - use IOboTime in Update(IOboTime t) methods instead
/// </summary>
public static class Time
{
    static IGameTime _oboTime;
    public static float TimeScale = 1f;

    /// <summary>
    /// if possible - use IOboTime in Update(IOboTime t) methods instead
    /// </summary>
    public static float Delta => _oboTime.Delta;
    /// <summary>
    /// if possible - use IOboTime in Update(IOboTime t) methods instead
    /// </summary>
    public static float DeltaScaled => _oboTime.Delta * TimeScale;
    /// <summary>
    /// if possible - use IOboTime in Update(IOboTime t) methods instead
    /// </summary>
    public static float TotalTime => _oboTime.TotalGameTime;

    public static IGameTime GetTimeManager() => _oboTime;
    
    public static void Init(IGameTime oboTime)
    {
        _oboTime = oboTime;
    }
}

public interface IGameTime
{
    float Delta { get; }
    float TotalGameTime { get; }
}

public class DefaultGameTime : IGameTime
{
    public float Delta => Engine.gt.Delta();
    public float TotalGameTime => (float)Engine.gt.TotalGameTime.TotalSeconds;
}