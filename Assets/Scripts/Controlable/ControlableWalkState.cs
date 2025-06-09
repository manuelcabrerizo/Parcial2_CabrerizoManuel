using UnityEngine;

public class ControlableWalkState : ControlableState
{
    public ControlableWalkState(Controlable controlable) 
        : base(controlable) { }


    public override void OnUpdate()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        ControlableData data = controlable.Data;
        // Apply gravity
        if (data.isGrounded == false)
        {
            data.forceAccumulator += new Vector3(0.0f, -9.8f * 3.0f, 0.0f);
        }

        Vector3 forward = data.cam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = data.cam.transform.right;
        right.y = 0;
        right.Normalize();

        data.direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            data.direction += forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            data.direction -= forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            data.direction += right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            data.direction -= right;
        }

        Ray groundRay = new Ray(data.body.position, Vector3.up * -1.0f);
        data.isGrounded = Physics.Raycast(groundRay, 0.75f);

        if (data.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            data.velocity.y = 0.0f;
            data.velocity += new Vector3(0.0f, 9.8f * 2.5f, 0.0f);
        }

        if (data.direction.sqrMagnitude > 0.0f)
        {
            data.direction.Normalize();
        }
        data.acceleration = data.direction * data.speed;

        Vector3 lastFrameAcceleration = Vector3.zero;

        if (CanMove(data.direction) || !data.isGrounded)
        {
            lastFrameAcceleration = data.acceleration;
        }
        else
        {
            data.velocity = new Vector3(data.body.velocity.x, 0.0f, data.body.velocity.z);
        }

        lastFrameAcceleration += data.forceAccumulator;
        data.velocity += lastFrameAcceleration * Time.deltaTime;
        data.velocity *= Mathf.Pow(data.damping, Time.deltaTime);

        data.body.velocity = data.velocity;
        data.body.transform.rotation = Quaternion.Euler(0.0f, data.cameraMovement.GetYaw(), 0.0f);

        if (data.animator != null)
        {
            data.animator.SetFloat("VelocityZ", Vector3.Dot(data.velocity, forward));
            data.animator.SetFloat("VelocityX", Vector3.Dot(data.velocity, right));
        }

        data.forceAccumulator = Vector3.zero;
    }

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
}
