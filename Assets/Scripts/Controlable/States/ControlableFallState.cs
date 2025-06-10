using System;
using UnityEngine;

public class ControlableFallState : ControlableState
{
    public ControlableFallState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }


    public override void OnEnter()
    {
        Debug.Log("FallState OnEnter");
    }

    public override void OnExit()
    {
        Debug.Log("FallState OnExit");
    }
}
