using System;
using UnityEngine;

public class ControlableFowardWalkState : ControlableState
{
    public ControlableFowardWalkState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = controlable.Data;
        data.body.drag = 5;
    }

    public override void OnExit()
    {
        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            data.animator.SetFloat("Velocity", 0);
        }
    }

    public override void OnUpdate()
    {
        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            Vector3 forward = data.body.transform.forward;
            Vector3 right = data.body.transform.right;
            data.animator.SetFloat("Velocity", Vector3.Dot(data.body.velocity, forward));
        }
    }

    public override void OnFixedUpdate()
    {
        ControlableData data = controlable.Data;

        Vector3 forward = data.cam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 direction = forward * Mathf.Clamp01(data.yInput);
        if (direction.sqrMagnitude > 1.0f)
        {
            direction.Normalize();
        }

        data.body.AddForce(direction * 30.0f, ForceMode.Force);

        Vector3 horizontalVel = data.body.velocity;
        horizontalVel.y = 0;
        if (horizontalVel.sqrMagnitude > (14.0f * 14.0f))
        {
            horizontalVel = horizontalVel.normalized * 14.0f;
        }
        horizontalVel.y = data.body.velocity.y;
        data.body.velocity = horizontalVel;
    }
}
