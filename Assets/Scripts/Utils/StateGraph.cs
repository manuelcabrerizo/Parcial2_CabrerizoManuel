using System.Collections.Generic;

public class StateGraph
{
    Dictionary<State, List<State>> stateTransitions;

    public StateGraph()
    {
        stateTransitions = new Dictionary<State, List<State>>();
    }

    public void AddStateTransitions(State state, List<State> states)
    {
        stateTransitions.Add(state, states);
    }

    public bool IsValid(State currentState, State tragetState)
    {
        if (stateTransitions.ContainsKey(currentState))
        {
            List<State> states = stateTransitions[currentState];
            foreach (var state in states)
            {
                if (state == tragetState)
                {
                    return true;
                }
            }
        }
        return false;
    }
}