using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlableData
{
    // Data
    public float xInput = 0.0f;
    public float yInput = 0.0f;
    public float moveDirLenSq = 0.0f;
    public bool isGrounded = false;

    // Compoenents
    public Rigidbody body = null;
    public CameraMovement cameraMovement = null;
    public Camera cam = null;
    public Player player = null;
    public Animator animator = null;
}

public class Controlable : MonoBehaviour
{
    public static event Action<GameObject> onControlableChange;

    private StateMachine stateMachine;
    private StateMachine additiveStateMachine;
    private StateGraph stateGraph = null;
    private List<ControlableState> basicStates = null;
    private List<ControlableState> additiveStates = null;

    public ControlableData Data { get; private set; }

    private void Awake()
    {
        Data = new ControlableData();
        Data.body = GetComponent<Rigidbody>();
        Data.animator = GetComponent<Animator>();
        Data.player = GetComponent<Player>();
        CameraMovement.onCameraCreate += OnCameraCreate;

        stateMachine = new StateMachine();
        additiveStateMachine = new StateMachine();
    }

    private void Start()
    {
        Player testPlayer = null;
        if (TryGetComponent<Player>(out testPlayer))
        {
            Player.InitControlable(this);
        }
        onControlableChange?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        CameraMovement.onCameraCreate -= OnCameraCreate;
        stateMachine.Clear();
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

    private void ProcessBreakFree()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Controlable newControlable = Data.player.gameObject.AddComponent<Controlable>();
            Player.InitControlable(newControlable);
            BreakFree();
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

    public void SetPlayer(Player player)
    {
        Data.player = player;
    }

    public void BreakFree()
    {
        Destroy(this);
    }
}
