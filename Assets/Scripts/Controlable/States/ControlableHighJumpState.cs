using System;
using UnityEngine;

public class ControlableHighJumpState : State<Controlable>
{
    public ControlableHighJumpState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = owner.DataSo.fallDrag;
        data.body.velocity = new Vector3(data.body.velocity.x, 0.0f, data.body.velocity.z);
        data.body.AddForce(Vector3.up * owner.DataSo.hightJumpForce, ForceMode.Impulse);
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
        if (direction.sqrMagnitude > 1.0f)
        {
            direction.Normalize();
        }

        data.body.AddForce(direction * owner.DataSo.fallHorizontalSpeed, ForceMode.Force);

        Vector3 horizontalVel = data.body.velocity;
        horizontalVel.y = 0;
        float maxVel = owner.DataSo.fallMaxHorizontalVel;
        if (horizontalVel.sqrMagnitude > (maxVel * maxVel))
        {
            horizontalVel = horizontalVel.normalized * maxVel;
        }
        horizontalVel.y = data.body.velocity.y;
        data.body.velocity = horizontalVel;
    }
}
