using System;
using UnityEngine;

public class ControlableJumpState : State<Controlable>
{
    private Player player = null;

    public ControlableJumpState(Controlable controlable, Func<bool> condition)
        : base(controlable, condition) 
    {
        controlable.TryGetComponent<Player>(out player);
    }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = owner.DataSo.fallDrag;
        data.body.velocity = new Vector3(data.body.velocity.x, 0.0f, data.body.velocity.z);
        data.body.AddForce(Vector3.up * owner.DataSo.jumpForce * Time.timeScale, ForceMode.Impulse);
        if (player != null && Time.timeScale > 0.0f)
        { 
            AudioManager.onPlayClip3D?.Invoke(player.Clips.jump, player.transform.position, 10, 40);
        }
        data.currentJumpDone++;
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
