using UnityEngine;

public class Face : SteeringBehavior
{
    public Face(Rigidbody character, Rigidbody target,
                float maxAngularAcceleration, float maxRotation,
                float targetRadius, float slowRadius,
                float timeToTarget)
    {
        this.character = character;
        this.target = target;
        this.maxAngularAcceleration = maxAngularAcceleration;
        this.maxRotation = maxRotation;
        this.targetRadius = targetRadius;
        this.slowRadius = slowRadius;
        this.timeToTarget = timeToTarget;
    }

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering = new SteeringOutput();

        Vector3 direction = target.position - character.position;
        if(direction.sqrMagnitude <= 0.01)
        {
            return steering;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        Quaternion currentRotation = character.rotation;

        // Calculate the rotation diffecrence
        Quaternion rotationDifference = targetRotation * Quaternion.Inverse(currentRotation);
        rotationDifference.ToAngleAxis(out float angle, out Vector3 axis);
        angle *= Mathf.Deg2Rad;
        if (angle > Mathf.PI)
        {
             angle -= Mathf.PI*2.0f;
        }
        if (angle < -Mathf.PI)
        {
             angle += Mathf.PI*2.0f;
        }

        float rotationSize = Mathf.Abs(angle);
        // Check if we are there, return no steering
        if(rotationSize < targetRadius)
        {
            return steering;
        }
        // if we are outside the slowRadius, then use the maxRotation
        float targetRotationSpeed;
        if(rotationSize > slowRadius)
        {
            targetRotationSpeed = maxRotation;
        }
        else
        {
            targetRotationSpeed = maxRotation * rotationSize / slowRadius;
        }

        // the final target rotation combines speed and direction
        Vector3 targetAngularVelocity = axis.normalized * targetRotationSpeed;
        Vector3 angularAcceleration = targetAngularVelocity - character.angularVelocity;
        angularAcceleration /= timeToTarget;

        // Check if the acceleration is to great
        if(angularAcceleration.magnitude > maxAngularAcceleration)
        {
            angularAcceleration = angularAcceleration.normalized * maxAngularAcceleration;
        }
        steering.angular = angularAcceleration;
        steering.linear = Vector3.zero;
        return steering;
    }
}