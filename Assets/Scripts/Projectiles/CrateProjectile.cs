using System.Collections;
using UnityEngine;

public class CrateProjectile : Projectile
{
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void Lunch(Vector3 startPosition, Vector3 targetPosition, float timeToTarget)
    {
        StartCoroutine(Lifetime());

        body.position = startPosition;
        body.velocity = Vector3.zero;

        Vector3 relPosition = targetPosition - startPosition;
        
        Vector3 up = Vector3.up;
        Vector3 right = relPosition;
        right.y = 0.0f;
        right.Normalize();

        float t = timeToTarget;
        float x0 = 0.0f;
        float y0 = 0.0f;
        float x = Vector3.Dot(relPosition, right);
        float y = targetPosition.y - startPosition.y;
        float v0x = (x - x0) / t;
        float v0y = (y - y0 - (0.5f * Physics.gravity.y * t * t)) / t;

        body.velocity = right * v0x + up * v0y; ;
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(20.0f);
        SendReleaseEvent();
    }
}
