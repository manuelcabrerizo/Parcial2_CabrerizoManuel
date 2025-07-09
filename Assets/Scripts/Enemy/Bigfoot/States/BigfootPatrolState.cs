using System;
using UnityEngine;

public class BigfootPatrolState : State<Bigfoot>
{
    private Transform target = null;

    public BigfootPatrolState(Bigfoot owner, Func<bool> enterCondition, Func<bool> exitCondition)
        : base(owner, enterCondition, exitCondition) { }

    public override void OnEnter()
    {
        owner.Animator.SetBool("IsAttaking", false);
        owner.Animator.SetBool("IsWalking", true);
        target = owner.PatrolPoints.GetClosest(owner.transform.position);
    }

    public override void OnExit()
    {
        owner.Animator.SetBool("IsWalking", false);
    }

    public override void OnUpdate()
    {
        float distance = (owner.transform.position - target.position).magnitude;
        if (distance <= 1.0f)
        {
            target = owner.PatrolPoints.GetNext();
        }
        AlignToTarget();
    }

    public override void OnFixedUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        float distance = (owner.transform.position - target.position).magnitude;
        if (distance > 1.0f)
        {
            owner.Body.AddForce(owner.transform.forward * 10.0f, ForceMode.Acceleration);
        }
    }

    private void AlignToTarget()
    {
        Vector3 forward = owner.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion currentRotation = Quaternion.LookRotation(forward, Vector3.up);

        forward = target.position - owner.transform.position;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

        Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime*0.75f);
        owner.transform.rotation = newRotation;
    }

}
