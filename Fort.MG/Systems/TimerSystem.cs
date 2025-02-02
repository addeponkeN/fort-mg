using Fort.MG.Utils;

namespace Fort.MG.Systems;

public class TimerSystem : EngineSystem
{
	private readonly List<FortTimer> _timers;
	private readonly HashSet<FortTimer> _hashedTimers;

    public TimerSystem()
    {
        _timers = new List<FortTimer>();
        _hashedTimers = new HashSet<FortTimer>();
    }
    
    public void Add(FortTimer timer)
    {
        if(_hashedTimers.Contains(timer))
            return;
        _timers.Add(timer);
        _hashedTimers.Add(timer);
    }

    public void Remove(FortTimer timer)
    {
        if(!_hashedTimers.Contains(timer))
            return;
        _timers.Remove(timer);
        _hashedTimers.Remove(timer);
    }
    
    public override void Update(IGameTime t)
    {
        base.Update(t);
        for(int i = 0; i < _timers.Count; i++)
        {
            _timers[i].Update(t);
            if(_timers[i].Timer < 0)
                Remove(_timers[i--]);
        }
    }
}