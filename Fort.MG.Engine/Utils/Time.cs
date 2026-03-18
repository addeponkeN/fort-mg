using Fort.MG.Extensions;

namespace Fort.MG.Utils;

/// <summary>
/// if possible - use IOboTime in Update(IOboTime t) methods instead
/// </summary>
public static class Time
{
    static IGameContext _oboContext;
    public static double TimeScale = 1.0;

    /// <summary>
    /// if possible - use IOboTime in Update(IOboTime t) methods instead
    /// </summary>
    public static double Delta => _oboContext.Delta;
    /// <summary>
    /// if possible - use IOboTime in Update(IOboTime t) methods instead
    /// </summary>
    public static double DeltaScaled => _oboContext.Delta * TimeScale;

    /// <summary>
    /// if possible - use IOboTime in Update(IOboTime t) methods instead
    /// </summary>
    public static double TotalTime => _oboContext.TotalGameTime;

    public static IGameContext GetTimeManager() => _oboContext;
    
    public static void Init(IGameContext oboContext)
    {
        _oboContext = oboContext;
    }
}

public class DefaultGameContext : IGameContext
{
    public float Delta => FortEngine.Time.Delta();
    public float TotalGameTime => FortEngine.Time.TotalSeconds();
    public double Delta64 => FortEngine.Time.Delta64();
    public double TotalGameTime64 => FortEngine.Time.TotalSeconds64();
}