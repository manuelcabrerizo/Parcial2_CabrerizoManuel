using UnityEngine;

public class BigfootIdleState : BigfootState
{
    public BigfootIdleState(Bigfoot bigfoot)
    : base(bigfoot) { }

    public override void OnEnter()
    {
        bigfoot.Animator.SetBool("IsAttaking", false);
    }

    public override void OnUpdate()
    {
        float distance = (bigfoot.Target.position - bigfoot.transform.position).magnitude;
        if (distance <= bigfoot.AttackRadio)
        {
            bigfoot.StateMachine.ChangeState(bigfoot.AttackState);
        }   
    }
}
