using System.Collections.Generic;

public class ControlableStateGraph
{
    Dictionary<ControlableType, StateGraph> stateGraphs;

    public ControlableStateGraph()
    { 
        stateGraphs = new Dictionary<ControlableType, StateGraph>();
    }

    public void AddGraph(ControlableType type, StateGraph graph)
    {
        stateGraphs.Add(type, graph);
    }

    public bool IsValid(ControlableType type, State currentState, State tragetState)
    {
        if (stateGraphs.ContainsKey(type))
        {
            StateGraph graph = stateGraphs[type];
            return graph.IsValid(currentState, tragetState);
        }
        return false;
    }
}
