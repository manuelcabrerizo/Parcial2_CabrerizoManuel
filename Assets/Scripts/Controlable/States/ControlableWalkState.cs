﻿using System;
using UnityEngine;

public class ControlableWalkState : State<Controlable>
{
    public ControlableWalkState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition) { }

    public override void OnEnter()
    {
        ControlableData data = owner.Data;
        data.body.drag = 5;
    }

    public override void OnExit()
    {
        ControlableData data = owner.Data;
        data.body.drag = 5;
        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", 0);
            data.animator.SetFloat("VelocityX", 0);
        }
    }

    public override void OnUpdate()
    {
        ControlableData data = owner.Data;
        if (data.animator != null)
        {
            Vector3 forward = data.body.transform.forward;
            Vector3 right = data.body.transform.right;
            data.animator.SetFloat("VelocityZ", Vector3.Dot(data.body.velocity, forward));
            data.animator.SetFloat("VelocityX", Vector3.Dot(data.body.velocity, right));
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

    /*
    private bool CanMove(Vector3 moveDir)
    {
        ControlableData data = controlable.Data;

        Terrain terrain = Terrain.activeTerrain;
        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(data.body.position);
        float nextHeight = terrain.SampleHeight(data.body.position + moveDir * 5);

        if ((angle > data.maxAngleMovement) && (nextHeight > currentHeight))
        {
            return false;
        }
        return true;
    }
    private Vector3 GetMapPos()
    {
        ControlableData data = controlable.Data;

        Vector3 pos = data.body.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }
    */
}
