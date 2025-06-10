using System;
using UnityEngine;

public class ControlableFallState : ControlableState
{
    public ControlableFallState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnFixedUpdate()
    {
        ControlableData data = controlable.Data;

        Vector3 forward = data.cam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = data.cam.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 direction = forward * data.yInput + right * data.xInput;
        if (direction.sqrMagnitude > 1.0f)
        {
            direction.Normalize();
        }

        data.body.AddForce(direction * 15.0f, ForceMode.Force);

        Vector3 horizontalVel = data.body.velocity;
        horizontalVel.y = 0;
        if (horizontalVel.sqrMagnitude > (4.5f * 4.5f))
        {
            horizontalVel = horizontalVel.normalized * 4.5f;
        }
        horizontalVel.y = data.body.velocity.y;
        data.body.velocity = horizontalVel;
    }
}
