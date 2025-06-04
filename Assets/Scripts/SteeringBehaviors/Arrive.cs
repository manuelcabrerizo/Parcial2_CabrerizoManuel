using UnityEngine;

public class Arrive : SteeringBehavior
{
    public Arrive(Rigidbody character, Rigidbody target,
                  float maxAcceleration, float maxSpeed,
                  float targetRadius, float slowRadius,
                  float timeToTarget)
    {
        this.character = character;
        this.target = target;
        this.maxAcceleration = maxAcceleration;
        this.maxSpeed = maxSpeed;
        this.targetRadius = targetRadius;
        this.slowRadius = slowRadius;
        this.timeToTarget = timeToTarget;
    }

    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering = new SteeringOutput();
        // Get the direction to target
        Vector3 direction = target.position - character.position;
        float distance = direction.magnitude;
        // Check if we are there, return no steering
        if(distance < targetRadius)
        {
            return steering;
        }
        // If we are outside the slowRadius, the go max Speed
        float targetSpeed;
        if(distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        }
        else
        {
            // Otherwise calculate a scale speed
            targetSpeed = maxSpeed * distance / slowRadius;
        }
        // Target velocity combines speed and direction
        Vector3 targetVelocity = direction.normalized;
        targetVelocity *= targetSpeed;
        // Acceleration tries to get to the target velocity
        steering.linear = targetVelocity - character.velocity;
        steering.linear /= timeToTarget;
        // Chack if the acceleration is too fast
        if(steering.linear.magnitude > maxAcceleration)
        {
            steering.linear = steering.linear.normalized * maxAcceleration;
        }
        steering.angular = Vector3.zero;
        return steering;
    }
}