using System;

public abstract class ControlableState : State
{
    private Func<bool> condition;
    protected Controlable controlable;

    public Func<bool> Condition => condition;

    public ControlableState(Controlable controlable, Func<bool> condition)
    {
        this.controlable = controlable;
        this.condition = condition;
    }
}
