using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlableData
{
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
    public static event Action<GameObject> onControlableChange;
    public ControlableData Data { get; private set; }
    private StateMachine stateMachine = null;
    private StateMachine additiveStateMachine = null;
    private StateGraph stateGraph = null;
    private List<ControlableState> basicStates = null;
    private List<ControlableState> additiveStates = null;

    private void Awake()
    {
        CameraMovement.onCameraCreate += OnCameraCreate;

        Data = new ControlableData();
        Data.body = GetComponent<Rigidbody>();
        Data.animator = GetComponent<Animator>();

        stateMachine = new StateMachine();
        additiveStateMachine = new StateMachine();
    }

    private void Start()
    {
        Initialize();
        onControlableChange?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        stateMachine.Clear();
        additiveStateMachine.Clear();

        CameraMovement.onCameraCreate -= OnCameraCreate;
    }

    private void Update()
    {
        ProcessRotation();
        ProcessControlableData();

        stateMachine.Update();
        additiveStateMachine.Update();

        ProcessBasicStates();
        ProcessAdditiveStates();
        ProcessBreakFree();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        additiveStateMachine.FixedUpdate();
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
        ControlableState idleState = new ControlableIdleState(this, () => { return Data.isGrounded && Data.moveDirLenSq <= 0.01f; });
        ControlableState walkState = new ControlableWalkState(this, () => { return Data.isGrounded && Data.moveDirLenSq > 0.01f; });
        ControlableState jumpState = new ControlableJumpState(this, () => { return Data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState fallState = new ControlableFallState(this, () => { return !Data.isGrounded && Data.body.velocity.y <= 0.0f; });

        StateGraph stateGraph = new StateGraph();
        stateGraph.AddStateTransitions(idleState, new List<State> { walkState, fallState, jumpState });
        stateGraph.AddStateTransitions(walkState, new List<State> { idleState, fallState, jumpState });
        stateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState });
        stateGraph.AddStateTransitions(jumpState, new List<State> { fallState });

        List<ControlableState> basicStates = new List<ControlableState> { idleState, walkState, fallState, jumpState };
        List<ControlableState> additiveStates = new List<ControlableState> { };

        SetStates(basicStates, additiveStates, stateGraph, idleState);
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
    }

    private void ProcessBasicStates()
    {
        foreach (ControlableState state in basicStates)
        {
            if (state.Condition() && stateMachine.PeekState() != state)
            {
                if (stateGraph.IsValid(stateMachine.PeekState() as State, state))
                {
                    stateMachine.ChangeState(state);
                }
            }
        }
    }

    private void ProcessAdditiveStates()
    {
        foreach (ControlableState state in additiveStates)
        {
            if (state.Condition())
            {
                if (additiveStateMachine.PeekState() != state)
                {
                    if (stateGraph.IsValid(stateMachine.PeekState() as State, state))
                    {
                        additiveStateMachine.PushState(state);
                    }
                }
            }
            else if (additiveStateMachine.PeekState() != null && additiveStateMachine.PeekState() == state)
            {
                if (stateGraph.IsValid(additiveStateMachine.PeekState() as State, stateMachine.PeekState() as State))
                {
                    additiveStateMachine.PopState();
                }
            }
        }
    }

    public void BreakFree()
    {
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

    public void SetStates(List<ControlableState> basicStates, List<ControlableState> additiveStates, StateGraph stateGraph, State initialState)
    {
        this.basicStates = basicStates;
        this.additiveStates = additiveStates;
        this.stateGraph = stateGraph;
        this.stateMachine.PushState(initialState);
    }

    public void SetPrevControlable(GameObject prevControlable)
    {
        Data.prevControlable = prevControlable;
    }
}
