using System;
using UnityEngine;

public class ControlableJumpState : ControlableState
{
    public ControlableJumpState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        Debug.Log("JumpState OnEnter");
        ControlableData data = controlable.Data;
        data.body.AddForce(Vector3.up * 8.0f, ForceMode.Impulse);
    }

    public override void OnExit()
    {
        Debug.Log("JumpState OnExit");
    }
}
