using System;
using UnityEngine;

public class BigfootIdleState : State<Bigfoot>
{
    public BigfootIdleState(Bigfoot owner, Func<bool> enterCondition, Func<bool> exitCondition)
        : base(owner, enterCondition, exitCondition) { }

    public override void OnEnter()
    {
        Debug.Log("Idle OnEnter");
        owner.Animator.SetBool("IsAttaking", false);
    }

    public override void OnExit()
    {
        Debug.Log("Idle OnExit");
    }
}
