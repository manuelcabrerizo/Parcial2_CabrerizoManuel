using System.Collections;
using UnityEngine;

public class CrateProjectile : Projectile
{
    [SerializeField] private LayerMask ignoreLayer;
    public Transform LaunchTransform { get; private set; }
    public Rigidbody Body { get; private set; }
    public Collider Collision { get; private set; }

    private bool canMakeDamage = false;

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        Collision = Body.GetComponent<Collider>();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Utils.CheckCollisionLayer(collision.gameObject, ignoreLayer))
        {
            canMakeDamage = false;
        }
    }

    public void Lunch(Vector3 startPosition, Vector3 targetPosition, Transform luchTransform, float timeToTarget)
    {
        StartCoroutine(Lifetime());

        canMakeDamage = true;
        this.LaunchTransform = luchTransform;

        Body.position = startPosition;
        Body.velocity = Vector3.zero;

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

        Body.velocity = right * v0x + up * v0y; ;
    }

    private IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(20.0f);
        SendReleaseEvent();
    }

    public bool CanMakeDamage()
    {
        return canMakeDamage;
    }
}
