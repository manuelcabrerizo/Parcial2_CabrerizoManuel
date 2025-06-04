using System.Collections.Generic;
using Unity.VisualScripting;

public class StateMachine
{
    private Stack<IState> states;
    public StateMachine()
    {
        states = new Stack<IState>();
    }

    public IState PeekState()
    {
        if (states.Count == 0) return null;
        return states.Peek();
    }

    public bool IsEmpty()
    {
        return states.Count == 0;
    }

    public void PushState(IState state)
    {
        state.OnEnter();
        states.Push(state);
    }

    public void PopState()
    {
        if (states.Count == 0) return;
        IState state = states.Pop();
        if (state != null)
        {
            state.OnExit();
        }
    }

    public void ChangeState(IState state)
    {
        if (states.Count > 0)
        {
            if (states.Peek() != state)
            {
                PopState();
                PushState(state);
            }
        }
        else
        {
            PushState(state);
        }
    }

    public void Clear()
    {
        while (states.Count > 0)
        {
            PopState();
        }
    }

    public void Update()
    {
        if (states.Count == 0) return;
        IState currentState = states.Peek();
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void FixedUpdate()
    {
        if (states.Count == 0) return;
        IState currentState = states.Peek();
        if (currentState != null)
        {
            currentState.OnFixedUpdate();
        }
    }
}