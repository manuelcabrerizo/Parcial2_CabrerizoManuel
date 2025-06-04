using UnityEngine;

public class SteeringOutput
{
    public Vector3 linear = Vector3.zero;
    public Vector3  angular = Vector3.zero;
}

public abstract class SteeringBehavior
{
    protected Rigidbody character = null;
    protected Rigidbody target = null;
    protected float maxAcceleration = 0.0f;
    protected float maxSpeed = 0.0f;
    protected float targetRadius = 0.0f;
    protected float slowRadius = 0.0f;
    protected float timeToTarget = 0.0f;
    protected float maxAngularAcceleration = 0.0f;
    protected float maxRotation = 0.0f;
    public abstract SteeringOutput GetSteering();

    public void SetTarget(Rigidbody target)
    {
        this.target = target;
    }
}