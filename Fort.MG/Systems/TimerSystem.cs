using Fort.MG.Utils;

namespace Fort.MG.Systems;

public class TimerSystem : EngineSystem
{
    List<FortTimer> timers;
    HashSet<FortTimer> hashedTimers;

    public TimerSystem()
    {
        timers = new List<FortTimer>();
        hashedTimers = new HashSet<FortTimer>();
    }
    
    public void Add(FortTimer timer)
    {
        if(hashedTimers.Contains(timer))
            return;
        timers.Add(timer);
        hashedTimers.Add(timer);
    }

    public void Remove(FortTimer timer)
    {
        if(!hashedTimers.Contains(timer))
            return;
        timers.Remove(timer);
        hashedTimers.Remove(timer);
    }
    
    public override void Update(IOboTime t)
    {
        base.Update(t);
        for(int i = 0; i < timers.Count; i++)
        {
            timers[i].Update(t);
            if(timers[i].Timer < 0)
                Remove(timers[i--]);
        }
    }
}