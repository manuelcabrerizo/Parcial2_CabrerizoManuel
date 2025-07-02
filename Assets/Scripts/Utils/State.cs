using System;
using UnityEngine;

public class State : IState
{
    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnUpdate()
    {
    }
    public virtual void OnFixedUpdate()
    {
    }
}

public class State<Type> : IState
{
    private Func<bool> enterCondition;
    private Func<bool> exitCondition;
    protected Type owner;

    public Func<bool> EnterCondition => enterCondition;
    public Func<bool> ExitCondition => exitCondition;

    public State(Type owner)
    {
        this.enterCondition = null;
        this.exitCondition = null;
        this.owner = owner;
    }

    public State(Type owner, Func<bool> enterCondition)
    {
        this.enterCondition = enterCondition;
        this.exitCondition = () => { return true; };
        this.owner = owner;
    }

    public State(Type owner, Func<bool> enterCondition, Func<bool> exitCondition)
    {
        this.enterCondition = enterCondition;
        this.exitCondition = exitCondition;
        this.owner = owner;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate() { }
    public virtual void OnFixedUpdate() { }
}
