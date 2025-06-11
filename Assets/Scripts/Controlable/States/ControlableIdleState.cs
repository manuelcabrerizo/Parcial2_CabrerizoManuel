using System;
using UnityEngine;

public class ControlableIdleState : ControlableState
{
    public ControlableIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }
}
