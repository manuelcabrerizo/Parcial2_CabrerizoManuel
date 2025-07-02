using System.Collections.Generic;

public class StateGraph<Type>
{
    private Dictionary<State<Type>, List<State<Type>>> stateTransitions;

    private List<State<Type>> basicStates = null;
    private List<State<Type>> additiveStates = null;
    private StateMachine basicStateMachine = null;
    private StateMachine additiveStateMachine = null;

    public StateGraph()
    {
        stateTransitions = new Dictionary<State<Type>, List<State<Type>>>();
        basicStateMachine = new StateMachine();
        additiveStateMachine = new StateMachine();
    }

    public void Update()
    { 
        basicStateMachine.Update();
        additiveStateMachine.Update();
        ProcessBasicStates();
        ProcessAdditiveStates();
    }

    public void FixedUpdate()
    {
        basicStateMachine.FixedUpdate();
        additiveStateMachine.FixedUpdate();
    }

    public void Clear()
    { 
        basicStateMachine.Clear();
        additiveStateMachine.Clear();
    }

    public void AddStateTransitions(State<Type> state, List<State<Type>> states)
    {
        stateTransitions.Add(state, states);
    }

    public bool IsValid(State<Type> currentState, State<Type> targetState)
    {
        if (stateTransitions.ContainsKey(currentState))
        {
            List<State<Type>> states = stateTransitions[currentState];
            foreach (var state in states)
            {
                if (state == targetState)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SetInitialState(State<Type> state)
    { 
        basicStateMachine.PushState(state);
    }

    public void AddBasicStates(List<State<Type>> states)
    { 
        this.basicStates = states;
    }

    public void AddAdditiveStates(List<State<Type>> states)
    {
        this.additiveStates = states;
    }

    private void ProcessBasicStates()
    {
        foreach (State<Type> state in basicStates)
        {
            State<Type> currentState = basicStateMachine.PeekState() as State<Type>;
            if (currentState.ExitCondition())
            {
                if (state.EnterCondition() && currentState != state)
                {
                    if (IsValid(currentState, state))
                    {
                        basicStateMachine.ChangeState(state);
                    }
                }
            }
        }
    }

    private void ProcessAdditiveStates()
    {
        foreach (State<Type> state in additiveStates)
        {
            if (state.EnterCondition())
            {
                if (additiveStateMachine.PeekState() != state)
                {
                    if (IsValid(basicStateMachine.PeekState() as State<Type>, state))
                    {
                        additiveStateMachine.PushState(state);
                    }
                }
            }
            else if (additiveStateMachine.PeekState() != null && additiveStateMachine.PeekState() == state)
            {
                if (IsValid(additiveStateMachine.PeekState() as State<Type>, basicStateMachine.PeekState() as State<Type>))
                {
                    additiveStateMachine.PopState();
                }
            }
        }
    }
}