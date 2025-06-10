using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlableData
{
    public float xInput;
    public float yInput;
    public float moveDirLenSq;

    public Rigidbody body;
    public Animator animator;
    public CameraMovement cameraMovement;
    public Camera cam;
    
    public float speed = 40.0f;
    public Vector3 direction = Vector3.zero;
    public bool isGrounded = false;
    public float maxAngleMovement = 30f;
}

public class Controlable : MonoBehaviour
{
    public static event Action<GameObject> onControlableChange;
    public static Player player = null;

    private StateMachine stateMachine;
    private StateMachine additiveStateMachine;
    private ControlableStateGraph stateGraph;
    private List<ControlableState> basicStates;
    private List<ControlableState> additiveStates;

    private ControlableData data;
    public ControlableData Data => data;

    [SerializeField] private ControlableType type;

    private void Awake()
    {
        if (player == null)
        {
            player = gameObject.GetComponent<Player>();
        }

        data = new ControlableData();
        data.body = GetComponent<Rigidbody>();
        data.animator = GetComponent<Animator>();
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
        stateMachine.Update();
        additiveStateMachine.Update();
        ProcessControlableData();
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
        ControlableIdleState idleState = new ControlableIdleState(this, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        ControlableWalkState walkState = new ControlableWalkState(this, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        ControlableJumpState jumpState = new ControlableJumpState(this, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableFallState fallState = new ControlableFallState(this, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });
        // Additive states
        ControlableSpellCastState spellCastState = new ControlableSpellCastState(this, () => { return Input.GetMouseButton(0); });

        StateGraph playerStateGraph = new StateGraph();
        playerStateGraph.AddStateTransitions(idleState, new List<State> { walkState, fallState, jumpState, spellCastState });
        playerStateGraph.AddStateTransitions(walkState, new List<State> { idleState, fallState, jumpState, spellCastState });
        playerStateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState, spellCastState });
        playerStateGraph.AddStateTransitions(jumpState, new List<State> { fallState, spellCastState });

        playerStateGraph.AddStateTransitions(spellCastState, new List<State> { idleState, walkState, jumpState, fallState });
        stateGraph.AddGraph(ControlableType.Player, playerStateGraph);

        StateGraph objectStateGraph = new StateGraph();
        objectStateGraph.AddStateTransitions(idleState, new List<State> { walkState });
        objectStateGraph.AddStateTransitions(walkState, new List<State> { idleState });
        stateGraph.AddGraph(ControlableType.Object, objectStateGraph);

        // set the initial state
        stateMachine.PushState(idleState);

        // Save basic states
        basicStates.Add(idleState);
        basicStates.Add(walkState);
        basicStates.Add(jumpState);
        basicStates.Add(fallState);
        // Save additive states
        additiveStates.Add(spellCastState);
    }

    private void OnCameraCreate(CameraMovement cam)
    {
        data.cameraMovement = cam;
        data.cam = data.cameraMovement.GetComponent<Camera>();
    }

    public void SetType(ControlableType type)
    {
        this.type = type;
    }

    public void BreakFree()
    {
        Destroy(this);
    }

    private void ProcessControlableData()
    {
        Ray groundRay = new Ray(data.body.position, Vector3.up * -1.0f);
        data.xInput = Input.GetAxis("Horizontal");
        data.yInput = Input.GetAxis("Vertical");
        data.moveDirLenSq = (data.xInput * data.xInput) + (data.yInput * data.yInput);
        data.isGrounded = Physics.Raycast(groundRay, 0.75f);
        data.body.useGravity = !data.isGrounded;
        data.body.drag = data.isGrounded ? 5 : 0;
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
                Controlable newControlable = player.gameObject.AddComponent<Controlable>();
                newControlable.SetType(ControlableType.Player);
                BreakFree();
            }
        }
    }
}
