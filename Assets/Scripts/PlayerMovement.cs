using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<PlayerMovement> onPlayerCreated;

    [SerializeField] LayerMask collisionLayer;
    private Animator animator;
    private SphereCollider sphereCollider;

    private CameraMovement cam;

    private float speed = 40.0f;
    private Vector3 direction = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;

    private Vector3 forceAccumulator = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private float damping = 0.01f;

    private Vector3 oldPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = 0.0f;
            velocity += new Vector3(0.0f, 9.8f * 1.5f, 0.0f);
        }

        if (direction.sqrMagnitude > 0.0f)
        {
            direction.Normalize();
        }
        acceleration = direction * speed;


        Vector3 lastFrameAcceleration = acceleration;
        lastFrameAcceleration += forceAccumulator;

        velocity += lastFrameAcceleration * Time.deltaTime;

        oldPosition = transform.position;
        transform.position += velocity * Time.deltaTime;

        CollisionDetectionAndResolution(velocity, 0);

        velocity *= Mathf.Pow(damping, Time.deltaTime);

        transform.rotation = Quaternion.Euler(0.0f, cam.GetYaw(), 0.0f);



        animator.SetFloat("VelocityZ", Vector3.Dot(velocity, forward));
        animator.SetFloat("VelocityX", Vector3.Dot(velocity, right));
        forceAccumulator = Vector3.zero;
    }

    private void CollisionDetectionAndResolution(Vector3 movement, int depth)
    {
        if (depth >= 5)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.SphereCast(oldPosition, sphereCollider.radius, movement.normalized, out hit, movement.magnitude * Time.deltaTime, collisionLayer))
        {
            transform.position = oldPosition + movement.normalized * hit.distance;
            transform.position += hit.normal * 0.01f;
            velocity -= hit.normal * Vector3.Dot(velocity, hit.normal);
            oldPosition = transform.position;
            CollisionDetectionAndResolution(velocity, depth + 1);
        }
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
