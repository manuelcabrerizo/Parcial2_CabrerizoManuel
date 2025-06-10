using System;
using UnityEngine;

public class ControlableIdleState : ControlableState
{
    public ControlableIdleState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        Debug.Log("IdleState OnEnter");

        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", 0);
            data.animator.SetFloat("VelocityX", 0);
        }
    }

    public override void OnExit()
    {
        Debug.Log("IdleState OnExit");
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

}
