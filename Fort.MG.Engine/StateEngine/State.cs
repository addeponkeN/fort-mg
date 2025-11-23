namespace Fort.MG.StateEngine;

public enum StateType
{
    None,
    Starting,
    Updating,
    Exiting,
}

public class State
{
    internal StateManager _manager;

    public StateType CurrentState { get; private set; }

    public float StartTime = 0.0f;
    public float ExitTime = 0.0f;
    public float Timer;

    public float StartProgress => Timer / StartTime;
    public float StartProgressInverted => (StartTime - Timer) / StartTime;
    public float ExitProgress => Timer / ExitTime;
    public float ExitProgressInverted => (ExitTime - Timer) / ExitTime;

    public virtual void Init() { }

    protected void SetState(State state)
    {
        _manager.SetState(state);
    }

    /// <summary>
    /// Step 1: Start
    /// </summary>
    public virtual void Start()
    {
        CurrentState = StateType.Starting;
        Timer = 0f;
    }

    /// <summary>
    /// Step 1: Update loop while starting
    /// </summary>
    public virtual void UpdateStarting(IGameTime t)
    {
        Timer += t.Delta;
    }

    /// <summary>
    /// Step 2: Starting is done, entering Update
    /// </summary>
    public virtual void OnUpdate()
    {
        CurrentState = StateType.Updating;
    }

    /// <summary>
    /// Step 2: The main update loop of the state
    /// </summary>
    public virtual void Update(IGameTime t)
    {
    }

    /// <summary>
    /// Step 3: Exit
    /// </summary>
    public virtual void Exit()
    {
        CurrentState = StateType.Exiting;
        Timer = 0f;
        _manager.OnStateExit(this);
    }

    /// <summary>
    /// During step 3 - Updating
    /// </summary>
    public virtual void UpdateExiting(IGameTime t)
    {
        Timer += t.Delta;
    }

    /// <summary>
    /// Step 3 finished - Exit state.
    /// </summary>
    public virtual void OnExited()
    {
    }
}