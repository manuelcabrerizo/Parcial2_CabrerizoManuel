using System;

public class BigfootIdleState : State<Bigfoot>
{
    public BigfootIdleState(Bigfoot owner, Func<bool> enterCondition, Func<bool> exitCondition)
        : base(owner, enterCondition, exitCondition) { }

    public override void OnEnter()
    {
        owner.Animator.SetBool("IsAttaking", false);
    }
}
