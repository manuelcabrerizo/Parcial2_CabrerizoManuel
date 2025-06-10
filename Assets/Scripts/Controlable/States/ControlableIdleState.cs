using System;
using UnityEngine;

public class ControlableIdleState : ControlableState
{
    public ControlableIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        Debug.Log("IdleState OnEnter");

        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", 0);
            data.animator.SetFloat("VelocityX", 0);
        }
    }

    public override void OnExit()
    {
        Debug.Log("IdleState OnExit");
    }
}
