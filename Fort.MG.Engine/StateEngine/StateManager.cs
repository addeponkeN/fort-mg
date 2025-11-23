namespace Fort.MG.StateEngine;

public class StateUpdater
{
    public void UpdateState<T>(IGameTime t, ref T state) where T : State
    {
        switch (state.CurrentState)
        {
            case StateType.Starting:
                state.UpdateStarting(t);
                if (state.StartProgress >= 1.0f)
                {
                    state.OnUpdate();
                }
                break;

            case StateType.Updating:
                state.Update(t);
                break;

            case StateType.Exiting:
                state.UpdateExiting(t);
                if (state.ExitProgress >= 1.0f)
                {
                    state.OnExited();
                    state = null;
                }
                break;
        }
    }
}

public class StateManager
{
    public State? Current;
    private State? _next;

    private readonly Type? _defaultStateType;
    private readonly StateUpdater _updater;

    public StateManager(Type? defaultState = null)
    {
        _defaultStateType = defaultState;
        _updater = new();
    }

    public virtual void SetState(State state)
    {
        state._manager = this;
        state.Init();
        if (Current != null)
        {
            Current.Exit();
            _next = state;
            return;
        }

        Current = state;
        Current.Start();
    }

    public virtual void OnStateExit(State newState)
    {
    }

    private void GoNextState()
    {
        if (_next == null)
        {
            if (_defaultStateType == null)
                return;
            var defState = (State)Activator.CreateInstance(_defaultStateType)!;
            SetState(defState);
            return;
        }

        Current = _next;
        Current.Start();

        _next = null;
    }

    public virtual void Update(IGameTime t)
    {
        if (Current == null)
        {
            GoNextState();
            return;
        }

        _updater.UpdateState(t, ref Current);
    }
}