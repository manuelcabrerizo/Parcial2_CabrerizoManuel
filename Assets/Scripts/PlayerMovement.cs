using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<PlayerMovement> onPlayerCreated;

    private Animator animator;
    private Rigidbody rb;

    private CameraMovement cam;

    private float speed = 40.0f;
    private Vector3 direction = Vector3.zero;
    private bool isGrounded = false;

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
        onPlayerCreated?.Invoke(this);
    }

    private void OnDestroy()
    {
        CameraMovement.onCameraCreate -= OnCameraCreate;
    }

    private void Update()
    {
        // Apply gravity
        forceAccumulator += new Vector3(0.0f, -9.8f*3.0f, 0.0f);

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

        Vector3 lastFrameAcceleration = acceleration;
        lastFrameAcceleration += forceAccumulator;

        velocity += lastFrameAcceleration * Time.deltaTime;
        velocity *= Mathf.Pow(damping, Time.deltaTime);

        Ray groundRay = new Ray(rb.position, Vector3.up * -1.0f);
        isGrounded = Physics.Raycast(groundRay, 0.501f);

        rb.velocity = velocity;

        transform.rotation = Quaternion.Euler(0.0f, cam.GetYaw(), 0.0f);

        animator.SetFloat("VelocityZ", Vector3.Dot(velocity, forward));
        animator.SetFloat("VelocityX", Vector3.Dot(velocity, right));
        forceAccumulator = Vector3.zero;


 
    }

    private void OnCameraCreate(CameraMovement cam)
    {
        this.cam = cam;
    }

    public void ClearMovement()
    {
        velocity = Vector3.zero;
    }
}
