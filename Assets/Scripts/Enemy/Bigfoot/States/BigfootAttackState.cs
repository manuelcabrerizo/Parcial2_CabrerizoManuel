using System;
using UnityEngine;

public class BigfootAttackState : State<Bigfoot>
{
    private CrateProjectile holdingCrate = null;

    public BigfootAttackState(Bigfoot owner, Func<bool> enterCondition, Func<bool> exitCondition)
        : base(owner, enterCondition, exitCondition) { }

    public override void OnEnter()
    {
        Bigfoot.onSpawnCrate += OnSpawnCrate;
        Bigfoot.onLunchCrate += OnLunchCrate;
        owner.Animator.SetBool("IsAttaking", true);
    }

    public override void OnExit()
    {
        Bigfoot.onSpawnCrate -= OnSpawnCrate;
        Bigfoot.onLunchCrate -= OnLunchCrate;
    }

    public override void OnUpdate()
    {
        FaceToTarget();
        SetHoldingCratePosition();
    }

    private void FaceToTarget()
    {
        Vector3 forward = owner.transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion currentRotation = Quaternion.LookRotation(forward, Vector3.up);

        forward = owner.Target.position - owner.transform.position;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

        Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 2.0f);
        owner.transform.rotation = newRotation;
    }

    private void SetHoldingCratePosition()
    {
        if (holdingCrate != null)
        {
            holdingCrate.transform.position = owner.Hand.position;
            holdingCrate.transform.rotation = owner.Hand.rotation;
        }
    }

    private void OnSpawnCrate(Bigfoot bigfoot)
    {
        if (bigfoot != owner)
        {
            return;
        }

        holdingCrate = ProjectileSpawner.Instance.Spawn<CrateProjectile>();
        holdingCrate.transform.position = owner.Hand.position;
        holdingCrate.transform.rotation = owner.Hand.rotation;
    }

    private void OnLunchCrate(Bigfoot bigfoot)
    {
        if (bigfoot != owner)
        {
            return;
        }

        if (holdingCrate != null)
        {
            float distance = (owner.Target.position - owner.transform.position).magnitude;
            float attackRadioRatio = Mathf.Min(distance / owner.AttackRadio, 1.0f);
            float timeToTarget = 2.0f - (2.0f * (1.0f - attackRadioRatio));
            holdingCrate.Lunch(holdingCrate.transform.position, owner.Target.position, timeToTarget);
            holdingCrate = null;
        }
    }
}
