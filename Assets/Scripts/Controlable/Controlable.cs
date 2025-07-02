using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlableData
{
    public float mousePosX = 0.0f;
    public float mousePosY = 0.0f;

    public float xInput = 0.0f;
    public float yInput = 0.0f;
    public float moveDirLenSq = 0.0f;
    public bool isGrounded = false;
    public Rigidbody body = null;
    public CameraMovement cameraMovement = null;
    public Camera cam = null;
    public GameObject prevControlable = null;
    public Animator animator = null;
}

public class Controlable : MonoBehaviour
{
    public static event Action<Controlable> onControlableCreated;
    public ControlableData Data { get; private set; }
    public StateGraph<Controlable> StateGraph { get; private set; }

    private void Awake()
    {
        CameraMovement.onCameraCreate += OnCameraCreate;

        StateGraph = new StateGraph<Controlable>();
        Data = new ControlableData();
        Data.body = GetComponent<Rigidbody>();
        Data.animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Initialize();
        onControlableCreated?.Invoke(this);
    }

    private void OnDestroy()
    {
        StateGraph.Clear();
        CameraMovement.onCameraCreate -= OnCameraCreate;
    }

    private void Update()
    {
        ProcessRotation();
        ProcessControlableData();
        StateGraph.Update();
        ProcessBreakFree();
    }

    private void FixedUpdate()
    {
        StateGraph.FixedUpdate();
    }

    private void Initialize()
    {
        CustomControlable customControlable = null;
        if (gameObject.TryGetComponent<CustomControlable>(out customControlable))
        {
            customControlable.Initialize(this);
        }
        else
        {
            SetDeafultControlable();
        }
    }

    private void SetDeafultControlable()
    {
        State<Controlable> idleState = new ControlableIdleState(this, () => { return Data.isGrounded && Data.moveDirLenSq <= 0.01f; });
        State<Controlable> walkState = new ControlableWalkState(this, () => { return Data.isGrounded && Data.moveDirLenSq > 0.01f; });
        State<Controlable> jumpState = new ControlableJumpState(this, () => { return Data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        State<Controlable> fallState = new ControlableFallState(this, () => { return !Data.isGrounded && Data.body.velocity.y <= 0.0f; });

        StateGraph.AddStateTransitions(idleState, new List<State<Controlable>> { walkState, fallState, jumpState });
        StateGraph.AddStateTransitions(walkState, new List<State<Controlable>> { idleState, fallState, jumpState });
        StateGraph.AddStateTransitions(fallState, new List<State<Controlable>> { idleState, walkState });
        StateGraph.AddStateTransitions(jumpState, new List<State<Controlable>> { fallState });

        List<State<Controlable>> basicStates = new List<State<Controlable>> { idleState, walkState, fallState, jumpState };
        List<State<Controlable>> additiveStates = new List<State<Controlable>> { };

        StateGraph.AddBasicStates(basicStates);
        StateGraph.AddAdditiveStates(additiveStates);
        StateGraph.SetInitialState(idleState);
    }

    private void ProcessRotation()
    {
        Vector3 forward = Data.body.transform.forward;
        Vector3 right = Data.body.transform.right;
        Data.body.transform.rotation = Quaternion.Euler(0.0f, Data.cameraMovement.GetYaw(), 0.0f);
    }

    private void ProcessControlableData()
    {
        Ray groundRay = new Ray(Data.body.position, Vector3.up * -1.0f);
        Data.xInput = Input.GetAxis("Horizontal");
        Data.yInput = Input.GetAxis("Vertical");
        Data.moveDirLenSq = (Data.xInput * Data.xInput) + (Data.yInput * Data.yInput);
        Data.isGrounded = Physics.Raycast(groundRay, 0.75f);
        if (Data.animator != null)
        {
            Data.animator.SetBool("IsGrounded", Data.isGrounded);
        }
        Data.body.useGravity = !Data.isGrounded;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float mouseSpeed = 8.0f;
        Data.mousePosX += mouseX * mouseSpeed;
        Data.mousePosY += mouseY * mouseSpeed;
        float radio = Screen.height * 0.4f;
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mousePos = new Vector2(Data.mousePosX, Data.mousePosY);
        if ((mousePos - center).sqrMagnitude > radio * radio)
        {
            mousePos = center + (mousePos - center).normalized * radio;
        }
        Data.mousePosX = mousePos.x;
        Data.mousePosY = mousePos.y;
    }

    public void BreakFree()
    {
        Data.body.useGravity = true;
        Destroy(this);
    }

    private void ProcessBreakFree()
    {
        if (Data.prevControlable != null)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Controlable newControlable = Data.prevControlable.AddComponent<Controlable>();
                newControlable.SetPrevControlable(this.gameObject);
                BreakFree();
            }
        }
    }

    private void OnCameraCreate(CameraMovement cam)
    {
        Data.cameraMovement = cam;
        Data.cam = Data.cameraMovement.GetComponent<Camera>();
    }

    public void SetPrevControlable(GameObject prevControlable)
    {
        Data.prevControlable = prevControlable;
    }
}
