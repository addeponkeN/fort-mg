using Fort.MG.Utils;

namespace Fort.MG.StateEngine;

public class StateManager
{
    public List<State> States;

    public StateManager()
    {
        States = new List<State>();
    }
    
    public virtual void AddState(State state)
    {
        state.StateManager = this;
        state.Init();
        States.Add(state);
    }

    public void SetState(State state)
    {
        for(int i = 0; i < States.Count; i++)
            RemoveState(States[i--]);
        AddState(state);
    }

    public void RemoveState(State st)
    {
        st.OnDestroyed();
        States.Remove(st);
    }

    public virtual void Update(IGameTime t)
    {
        for(int i = 0; i < States.Count; i++)
        {
            var st = States[i];
            if(!st.Started)
            {
                st.Start();
                st.Started = true;
            }
            if(st.IsAlive)
                st.Update(t);
            if(!st.IsAlive)
                RemoveState(st);
        }
    }
    
}