using System;
using UnityEngine;

public class ControlableWalkState : State<Controlable>
{
    public ControlableWalkState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = 5;
        data.currentJumpDone = 0;
    }

    public override void OnExit()
    {
        ControlableData data = owner.Data;
        data.body.drag = 5;
    }

    public override void OnUpdate()
    {
        ControlableData data = owner.Data;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityX", data.smoothXInput*0.5f);
            data.animator.SetFloat("VelocityZ", data.smoothYInput*0.5f);
        }
    }

    public override void OnFixedUpdate()
    {
        ControlableData data = owner.Data;

        Vector3 forward = data.cam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = data.cam.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 direction = forward * data.yInput + right * data.xInput;
        if (owner.CanMove(direction))
        {
            if (direction.sqrMagnitude > 1.0f)
            {
                direction.Normalize();
            }
            data.body.AddForce(direction * 30.0f, ForceMode.Force);
        }

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
