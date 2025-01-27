using Fort.MG.Utils;

namespace Fort.MG.StateEngine;

public class State
{
    public StateManager StateManager { get; set; }
    public bool IsAlive = true;

    internal bool Started;

    public virtual void Init()
    {
    }

    public virtual void Start()
    {
        Started = true;
    }

    public virtual void Update(IGameTime t)
    {
    }

    public void Kill()
    {
        IsAlive = false;
    }

    public virtual void OnDestroyed()
    {
    }
    
}