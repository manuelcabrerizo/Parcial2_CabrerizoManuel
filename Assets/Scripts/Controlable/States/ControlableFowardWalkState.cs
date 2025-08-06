using System;
using UnityEngine;

public class ControlableFowardWalkState : State<Controlable>
{
    public ControlableFowardWalkState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = owner.DataSo.normalDrag;
    }

    public override void OnExit()
    {
        ControlableData data = owner.Data;
    }

    public override void OnUpdate()
    {
        ControlableData data = owner.Data;
        if (data.animator != null)
        {
            Vector3 forward = data.body.transform.forward;
            Vector3 right = data.body.transform.right;
            data.animator.SetFloat("Velocity", Mathf.Clamp01(data.yInput));
        }
    }

    public override void OnFixedUpdate()
    {
        ControlableData data = owner.Data;

        Vector3 forward = data.cam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 direction = forward * Mathf.Clamp01(data.yInput);
        if (direction.sqrMagnitude > 1.0f)
        {
            direction.Normalize();
        }

        data.body.AddForce(direction * owner.DataSo.fowardWalkSpeed, ForceMode.Acceleration);

        Vector3 horizontalVel = data.body.velocity;
        horizontalVel.y = 0;
        float maxVel = owner.DataSo.fowardWalkMaxVelocity;
        if (horizontalVel.sqrMagnitude > (maxVel * maxVel))
        {
            horizontalVel = horizontalVel.normalized * maxVel;
        }
        horizontalVel.y = data.body.velocity.y;
        data.body.velocity = horizontalVel;
    }
}
