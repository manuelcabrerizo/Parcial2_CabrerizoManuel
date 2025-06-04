using UnityEngine;

public class Bigfoot : Enemy
{
    [SerializeField] private float attackRadio = 4.0f;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform target;

    private CrateProjectile holdingCrate = null;
    private float attackRadioRatio;

    private void Update()
    {
        CalculateAttackRadioRatio();
        FaceToTarget();
        SetHoldingCratePosition();
    }

    private void CalculateAttackRadioRatio()
    {
        float distance = (target.position - transform.position).magnitude;
        attackRadioRatio = Mathf.Min(distance / attackRadio, 1.0f); 
    }

    private void FaceToTarget()
    {
        Vector3 forward = transform.forward;
        forward.y = 0.0f;
        forward.Normalize();
        Quaternion currentRotation = Quaternion.LookRotation(forward, Vector3.up);

        forward = target.position - transform.position;
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



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadio);
    }

    public void SpawnCrate()
    {
        holdingCrate = ProjectileSpawner.Instance.Spawn<CrateProjectile>();
        holdingCrate.transform.position = hand.position;
        holdingCrate.transform.rotation = hand.rotation;
    }

    public void LunchCrate()
    {
        holdingCrate.Lunch(holdingCrate.transform.position, target.position, 2.0f - (2.0f * (1.0f - attackRadioRatio)));
        holdingCrate = null;
    }

}
