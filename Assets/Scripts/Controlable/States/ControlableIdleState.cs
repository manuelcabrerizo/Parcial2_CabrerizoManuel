using System;
using UnityEngine;

public class ControlableIdleState : State<Controlable>
{
    public ControlableIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }
}
