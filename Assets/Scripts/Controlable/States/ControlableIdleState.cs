using System;

public class ControlableIdleState : State<Controlable>
{
    public ControlableIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }
}


public class PlayerIdleState : State<Controlable>
{
    public PlayerIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = 5;
        data.currentJumpDone = 0;
    }

    public override void OnUpdate()
    {
        ControlableData data = owner.Data;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityX", data.smoothXInput * 0.5f);
            data.animator.SetFloat("VelocityZ", data.smoothYInput * 0.5f);
        }
    }
}
