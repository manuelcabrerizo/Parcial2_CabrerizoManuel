using System;
using UnityEngine;

public class ControlableWalkState : State<Controlable>
{
    private bool sprint = false;

    public ControlableWalkState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = 5;
        data.currentJumpDone = 0;
        sprint = false;
        if (data.animator != null)
        {
            data.animator.SetBool("IsSprinting", sprint);
        }
    }

    public override void OnExit()
    {
        sprint = false;
        ControlableData data = owner.Data;
        if (data.animator != null)
        {
            data.animator.SetBool("IsSprinting", sprint);
        }
    }

    public override void OnUpdate()
    {
        ControlableData data = owner.Data;

        //sprint = Input.GetKey(KeyCode.LeftShift);
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityX", data.smoothXInput*0.5f);
            data.animator.SetFloat("VelocityZ", data.smoothYInput*0.5f);
            data.animator.SetBool("IsSprinting", sprint);
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

        Ray downRay = new Ray(data.body.position, Vector3.up * -1.0f);
        RaycastHit hit;
        Physics.Raycast(downRay, out hit);

        float speed = sprint ? 80.0f : 40.0f;

        Vector3 normal = hit.normal;
        Plane walkPlane = new Plane(normal, 0);
        Vector3 planeDir = walkPlane.ClosestPointOnPlane(direction);
        direction = planeDir.normalized;
        if (owner.CanMove(direction))
        {
            if (direction.sqrMagnitude > 1.0f)
            {
                direction.Normalize();
            }
            data.body.AddForce(direction * speed, ForceMode.Force);
        }
    }    
}
