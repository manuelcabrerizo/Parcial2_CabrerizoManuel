using System;
using System.Collections.Generic;
using UnityEngine;

public class ControlableData
{
    public Rigidbody body;
    public Animator animator;
    public CameraMovement cameraMovement;
    public Camera cam;
    public float speed = 40.0f;
    public Vector3 direction = Vector3.zero;
    public bool isGrounded = false;
    public float maxAngleMovement = 30f;
    public Vector3 forceAccumulator = Vector3.zero;
    public Vector3 acceleration = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public float damping = 0.01f;
}

public class Controlable : MonoBehaviour
{
    public static event Action<GameObject> onControlableChange;
    public static Player player = null;

    private ControlableIdleState idleState;
    private ControlableWalkState walkState;
    private ControlableSpellCastState spellCastState;

    private StateMachine stateMachine;
    private StateMachine additiveStateMachine;

    private ControlableStateGraph stateGraph;

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

        idleState = new ControlableIdleState(this);
        walkState = new ControlableWalkState(this);
        spellCastState = new ControlableSpellCastState(this);
        stateGraph = new ControlableStateGraph();
        stateMachine = new StateMachine();
        additiveStateMachine = new StateMachine();
    }

    private void Start()
    {
        InitializeStateGraph();
        stateMachine.PushState(idleState);

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

        float xmovement = Input.GetAxis("Horizontal");
        float ymovement = Input.GetAxis("Vertical");
        float lenSq = (xmovement * xmovement) + (ymovement * ymovement);
        if (lenSq > 0.01f)
        {
            if (stateGraph.IsValid(type, stateMachine.PeekState() as State, walkState))
            {
                stateMachine.ChangeState(walkState);
            }
        }
        else
        {
            if (stateGraph.IsValid(type, stateMachine.PeekState() as State, idleState))
            {
                stateMachine.ChangeState(idleState);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (stateGraph.IsValid(type, stateMachine.PeekState() as State, spellCastState))
            {
                additiveStateMachine.PushState(spellCastState);
            }
        }

        if (additiveStateMachine.PeekState() != null && Input.GetMouseButtonUp(0))
        {
            if (stateGraph.IsValid(type, additiveStateMachine.PeekState() as State, stateMachine.PeekState() as State))
            {
                additiveStateMachine.PopState();
            }
        }

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

    private void InitializeStateGraph()
    {
        StateGraph playerStateGraph = new StateGraph();
        playerStateGraph.AddStateTransitions(idleState, new List<State> { walkState, spellCastState });
        playerStateGraph.AddStateTransitions(walkState, new List<State> { idleState, spellCastState });
        playerStateGraph.AddStateTransitions(spellCastState, new List<State> { idleState, walkState });
        stateGraph.AddGraph(ControlableType.Player, playerStateGraph);

        StateGraph objectStateGraph = new StateGraph();
        objectStateGraph.AddStateTransitions(idleState, new List<State> { walkState });
        objectStateGraph.AddStateTransitions(walkState, new List<State> { idleState });
        stateGraph.AddGraph(ControlableType.Object, objectStateGraph);
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
}
