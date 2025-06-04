using UnityEngine;

public class Flee : SteeringBehavior
{
    public Flee(Rigidbody character, Rigidbody target, float maxAcceleration)
    {
        this.character = character;
        this.target = target;
        this.maxAcceleration = maxAcceleration;
    }
    public override SteeringOutput GetSteering()
    {
        SteeringOutput steering = new SteeringOutput();
        // Get the direction to the target
        steering.linear = character.position - target.position;
        // Give full acceleration along this direction
        steering.linear.Normalize();
        steering.linear *= maxAcceleration;
        steering.angular = Vector3.zero;
        return steering;
    }
}