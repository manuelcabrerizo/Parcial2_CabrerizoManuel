using System;
using UnityEngine;

public class EntityMovement : MonoBehaviour
{
    public static event Action<EntityMovement> onEntityCreated;

    private Animator animator;
    private Rigidbody rb;

    private CameraMovement cameraMovement;
    private Camera cam;

    private float speed = 40.0f;
    private Vector3 direction = Vector3.zero;
    private bool isGrounded = false;
    private float maxAngleMovement = 30f;

    private Vector3 forceAccumulator = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private float damping = 0.01f;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        CameraMovement.onCameraCreate += OnCameraCreate;
    }

    private void Start()
    {
        onEntityCreated?.Invoke(this);
    }

    private void OnDestroy()
    {
        CameraMovement.onCameraCreate -= OnCameraCreate;
    }

    private void Update()
    {
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        // Apply gravity
        if (isGrounded == false)
        {
            forceAccumulator += new Vector3(0.0f, -9.8f * 3.0f, 0.0f);
        }

        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = cam.transform.right;
        right.y = 0;
        right.Normalize();

        direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            direction += forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction -= right;
        }

        Ray groundRay = new Ray(rb.position, Vector3.up * -1.0f);
        isGrounded = Physics.Raycast(groundRay, 0.75f);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = 0.0f;
            velocity += new Vector3(0.0f, 9.8f * 2.5f, 0.0f);
        }

        if (direction.sqrMagnitude > 0.0f)
        {
            direction.Normalize();
        }
        acceleration = direction * speed;

        Vector3 lastFrameAcceleration = Vector3.zero;

        if (CanMove(direction) || !isGrounded)
        {
            lastFrameAcceleration = acceleration;
        }
        else
        {
            velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
        }

        lastFrameAcceleration += forceAccumulator;
        velocity += lastFrameAcceleration * Time.deltaTime;
        velocity *= Mathf.Pow(damping, Time.deltaTime);

        rb.velocity = velocity;
        transform.rotation = Quaternion.Euler(0.0f, cameraMovement.GetYaw(), 0.0f);

        if (animator != null)
        {
            animator.SetFloat("VelocityZ", Vector3.Dot(velocity, forward));
            animator.SetFloat("VelocityX", Vector3.Dot(velocity, right));
        }

        forceAccumulator = Vector3.zero;
    }

    private bool CanMove(Vector3 moveDir)
    {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * 5);
        Debug.Log(angle);
        if ((angle > maxAngleMovement) && (nextHeight > currentHeight))
        {
            return false;
        }
        return true;
    }

    private Vector3 GetMapPos()
    {
        Vector3 pos = rb.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }

    private void OnCameraCreate(CameraMovement cam)
    {
        this.cameraMovement = cam;
        this.cam = cameraMovement.GetComponent<Camera>();
    }

    public void ClearMovement()
    {
        velocity = Vector3.zero;
    }
}
