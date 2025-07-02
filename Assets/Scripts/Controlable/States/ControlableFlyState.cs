using System;
using UnityEngine;

public class ControlableFlyState : State<Controlable>
{
    public ControlableFlyState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        Debug.Log("Fly OnEnter");
        ControlableData data = owner.Data;
        data.body.drag = 2.5f;
        data.body.velocity = new Vector3(data.body.velocity.x, 0.0f, data.body.velocity.z);
        data.isGrounded = false;
        // temp fix
        data.body.position += Vector3.up;
    }

    public override void OnExit()
    {
        Debug.Log("Fly OnExit");
        ControlableData data = owner.Data;
        data.body.useGravity = true;
    }

    public override void OnUpdate()
    {
        ControlableData data = owner.Data;
        data.body.useGravity = false;
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

        float upInput = 0.0f;
        if (Input.GetKey(KeyCode.Space))
        {
            upInput = 1.0f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            upInput = -1.0f;
        }

        Vector3 direction = forward * data.yInput + right * data.xInput + Vector3.up * upInput;
        if (direction.sqrMagnitude > 1.0f)
        {
            direction.Normalize();
        }

        data.body.AddForce(direction * 30.0f, ForceMode.Force);

        Vector3 velocity = data.body.velocity;
        if (velocity.sqrMagnitude > (14.0f * 14.0f))
        {
            velocity = velocity.normalized * 14.0f;
        }
        data.body.velocity = velocity;
    }

}
