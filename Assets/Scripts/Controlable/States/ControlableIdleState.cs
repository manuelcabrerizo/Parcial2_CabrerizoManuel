using System;
using UnityEngine;

public class ControlableIdleState : ControlableState
{
    public ControlableIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = controlable.Data;
        data.body.drag = 5;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", 0);
            data.animator.SetFloat("VelocityX", 0);
        }
    }
}
