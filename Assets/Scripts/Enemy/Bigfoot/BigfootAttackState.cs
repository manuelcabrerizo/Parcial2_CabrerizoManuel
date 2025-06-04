using UnityEngine;

public class BigfootAttackState : MonoBehaviourState
{
    [SerializeField] private Transform hand;

    private Bigfoot bigfoot = null;
    private CrateProjectile holdingCrate = null;
    private float attackRadioRatio;

    public override void OnUpdate()
    {
        CalculateAttackRadioRatio();
        FaceToTarget();
        SetHoldingCratePosition();
    }

    private void CalculateAttackRadioRatio()
    {
        float distance = (bigfoot.Target.position - transform.position).magnitude;
        attackRadioRatio = Mathf.Min(distance / bigfoot.AttackRadio, 1.0f);
    }

    private void FaceToTarget()
    {
        Vector3 forward = transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion currentRotation = Quaternion.LookRotation(forward, Vector3.up);

        forward = bigfoot.Target.position - transform.position;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

        Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * 2.0f);
        transform.rotation = newRotation;
    }

    private void SetHoldingCratePosition()
    {
        if (holdingCrate != null)
        {
            holdingCrate.transform.position = hand.position;
            holdingCrate.transform.rotation = hand.rotation;
        }
    }

    public void SpawnCrate()
    {
        holdingCrate = ProjectileSpawner.Instance.Spawn<CrateProjectile>();
        holdingCrate.transform.position = hand.position;
        holdingCrate.transform.rotation = hand.rotation;
    }

    public void LunchCrate()
    {
        float timeToTarget = 2.0f - (2.0f * (1.0f - attackRadioRatio));
        holdingCrate.Lunch(holdingCrate.transform.position, bigfoot.Target.position, timeToTarget);
        holdingCrate = null;
    }

    public void SetBigfoot(Bigfoot bigfoot)
    {
        this.bigfoot = bigfoot;
    }
}
