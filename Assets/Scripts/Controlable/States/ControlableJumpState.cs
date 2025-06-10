using System;
using UnityEngine;

public class ControlableJumpState : ControlableState
{
    public ControlableJumpState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        Debug.Log("JumpState OnEnter");
        ControlableData data = controlable.Data;
        data.body.AddForce(Vector3.up * 8.0f, ForceMode.Impulse);
    }

    public override void OnExit()
    {
        Debug.Log("JumpState OnExit");
    }

    public override void OnUpdate()
    {
        ControlableData data = controlable.Data;

        Vector3 forward = data.body.transform.forward;
        Vector3 right = data.body.transform.right;
        data.body.transform.rotation = Quaternion.Euler(0.0f, data.cameraMovement.GetYaw(), 0.0f);
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", Vector3.Dot(data.body.velocity, forward));
            data.animator.SetFloat("VelocityX", Vector3.Dot(data.body.velocity, right));
        }
    }

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
