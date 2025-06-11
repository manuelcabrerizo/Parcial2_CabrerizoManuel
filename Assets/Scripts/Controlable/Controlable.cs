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
    private ControlableStateGraph stateGraph;
    private List<ControlableState> basicStates;
    private List<ControlableState> additiveStates;

    public ControlableData Data { get; private set; }

    [SerializeField] private ControlableType type;

    private void Awake()
    {
        Data = new ControlableData();
        Data.body = GetComponent<Rigidbody>();
        Data.animator = GetComponent<Animator>();
        Data.player = GetComponent<Player>();
        CameraMovement.onCameraCreate += OnCameraCreate;

        stateMachine = new StateMachine();
        additiveStateMachine = new StateMachine();
        stateGraph = new ControlableStateGraph();
        basicStates = new List<ControlableState>();
        additiveStates = new List<ControlableState>();
    }

    private void Start()
    {
        SetupStates();
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

    private void SetupStates()
    {
        // Basic states
        ControlableState idleState = new ControlableIdleState(this, () => { return Data.isGrounded && Data.moveDirLenSq <= 0.01f; });
        ControlableState walkState = new ControlableWalkState(this, () => { return Data.isGrounded && Data.moveDirLenSq > 0.01f; });
        ControlableState fowardWalkState = new ControlableFowardWalkState(this, () => { return Data.isGrounded && Data.moveDirLenSq > 0.01f; });
        ControlableState jumpState = new ControlableJumpState(this, () => { return Data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState highJumpState = new ControlableHighJumpState(this, () => { return Data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState flyState = new ControlableFlyState(this, () => { return Data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState fallState = new ControlableFallState(this, () => { return !Data.isGrounded && Data.body.velocity.y <= 0.0f; });
        // Additive states
        ControlableState spellCastState = new ControlableSpellCastState(this, () => { return Input.GetMouseButton(0); });

        StateGraph playerStateGraph = new StateGraph();
        playerStateGraph.AddStateTransitions(idleState, new List<State> { walkState, fallState, jumpState, spellCastState });
        playerStateGraph.AddStateTransitions(walkState, new List<State> { idleState, fallState, jumpState, spellCastState });
        playerStateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState, spellCastState });
        playerStateGraph.AddStateTransitions(jumpState, new List<State> { fallState, spellCastState });
        playerStateGraph.AddStateTransitions(spellCastState, new List<State> { idleState, walkState, jumpState, fallState });
        stateGraph.AddGraph(ControlableType.Player, playerStateGraph);

        StateGraph bunnyStateGraph = new StateGraph();
        bunnyStateGraph.AddStateTransitions(idleState, new List<State> { fowardWalkState, fallState, highJumpState });
        bunnyStateGraph.AddStateTransitions(fowardWalkState, new List<State> { idleState, fallState, highJumpState });
        bunnyStateGraph.AddStateTransitions(fallState, new List<State> { idleState, fowardWalkState });
        bunnyStateGraph.AddStateTransitions(highJumpState, new List<State> { fallState });
        stateGraph.AddGraph(ControlableType.Bunny, bunnyStateGraph);

        StateGraph dragonStateGraph = new StateGraph();
        dragonStateGraph.AddStateTransitions(idleState, new List<State> { walkState, flyState, fallState });
        dragonStateGraph.AddStateTransitions(walkState, new List<State> { idleState, flyState, fallState });
        dragonStateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState });
        dragonStateGraph.AddStateTransitions(flyState, new List<State> { idleState, walkState });
        stateGraph.AddGraph(ControlableType.Dragon, dragonStateGraph);

        StateGraph objectStateGraph = new StateGraph();
        objectStateGraph.AddStateTransitions(idleState, new List<State> { walkState, fallState, jumpState });
        objectStateGraph.AddStateTransitions(walkState, new List<State> { idleState, fallState, jumpState });
        objectStateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState });
        objectStateGraph.AddStateTransitions(jumpState, new List<State> { fallState });
        stateGraph.AddGraph(ControlableType.Object, objectStateGraph);

        // set the initial state
        stateMachine.PushState(idleState);

        // Save basic states
        basicStates.Add(idleState);
        basicStates.Add(walkState);
        basicStates.Add(fowardWalkState);
        basicStates.Add(jumpState);
        basicStates.Add(highJumpState);
        basicStates.Add(flyState);
        basicStates.Add(fallState);
        // Save additive states
        additiveStates.Add(spellCastState);
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
                if (stateGraph.IsValid(type, stateMachine.PeekState() as State, state))
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
                    if (stateGraph.IsValid(type, stateMachine.PeekState() as State, state))
                    {
                        additiveStateMachine.PushState(state);
                    }
                }
            }
            else if (additiveStateMachine.PeekState() != null && additiveStateMachine.PeekState() == state)
            {
                if (stateGraph.IsValid(type, additiveStateMachine.PeekState() as State, stateMachine.PeekState() as State))
                {
                    additiveStateMachine.PopState();
                }
            }
        }
    }

    private void ProcessBreakFree()
    {
        if (type != ControlableType.Player)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Controlable newControlable = Data.player.gameObject.AddComponent<Controlable>();
                newControlable.SetType(ControlableType.Player);
                BreakFree();
            }
        }
    }

    private void OnCameraCreate(CameraMovement cam)
    {
        Data.cameraMovement = cam;
        Data.cam = Data.cameraMovement.GetComponent<Camera>();
    }

    public void SetType(ControlableType type)
    {
        this.type = type;
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
