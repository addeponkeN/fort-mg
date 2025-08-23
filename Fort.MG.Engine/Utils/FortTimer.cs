using Fort.MG.Systems;

namespace Fort.MG.Utils;

public class FortTimer
{
	public float Interval { get; set; }
	public float Timer { get; internal set; }

	public bool Loop { get; set; }

    public bool TimeScaled;
    public bool IsRunning;
    
    public event Action OnTick;

    public float Lerp => 1f - Timer / Interval;
    public float LerpReverse => Timer / Interval;

    public FortTimer(float interval)
    {
        Interval = interval;
        Timer = interval;
    }

    public static implicit operator FortTimer(float time) => new FortTimer(time);

    public void Start()
    {
        if(IsRunning) return;
        
        FortEngine.SystemManager.Get<TimerSystem>().Add(this);
        Timer = Interval;
        IsRunning = true;
    }

    public void Stop()
    {
        IsRunning = false;
        Timer = -1;
    }

    /// <returns>true when time is done hehe</returns>
    public void Update(IGameTime t)
    {
        if(!IsRunning) return;

        Timer -= t.Delta;

        if(Timer <= 0)
        {
            OnTick?.Invoke();
            if (Loop)
            {
	            Timer += Interval;
            }
        }
    }
}