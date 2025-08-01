using System;
using System.Collections.Generic;
using UnityEngine;

public class Controlable : MonoBehaviour
{
    public static event Action<Controlable> onControlableCreated;
    public static event Action<Controlable> onControlableBreakFree;
    public ControlableData Data { get; private set; }
    public StateGraph<Controlable> StateGraph { get; private set; }
    [field:SerializeField] public ControlableDataSO DataSo { get; set; } 
    [SerializeField] private float maxAngleMovement = 30.0f;
    private LayerMask ignoreGroundRay;

    private bool isPause = false;
    public bool IsPause => isPause;


    private void Awake()
    {
        CameraMovement.onCameraCreate += OnCameraCreate;
        PauseState.onPauseStateEnter += OnPauseEnter;
        PauseState.onPauseStateExit += OnPauseExit;
        EndState.onEndStateEnter += OnEndStateEnter;
        EndState.onEndStateExit += OnEndStateExit;

        StateGraph = new StateGraph<Controlable>();
        Data = new ControlableData();
        Data.body = GetComponent<Rigidbody>();
        Data.animator = GetComponent<Animator>();

        ignoreGroundRay = LayerMask.NameToLayer("IgnoreGroundRay");

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
        PauseState.onPauseStateEnter -= OnPauseEnter;
        PauseState.onPauseStateExit -= OnPauseExit;
        EndState.onEndStateEnter -= OnEndStateEnter;
        EndState.onEndStateExit -= OnEndStateExit;
    }

    private void Update()
    {
        if (isPause) return;

        ProcessControlableData();
        StateGraph.Update();
        ProcessBreakFree();
    }

    private void FixedUpdate()
    {
        if (isPause) return;

        StateGraph.FixedUpdate();
    }

    private void LateUpdate()
    {
        if (isPause) return;

        ProcessRotation();
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
    {   Vector3 forward = Data.cam.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Data.body.transform.rotation = Quaternion.LookRotation(forward);
    }

    private void ProcessControlableData()
    {
        Ray groundRay = new Ray(Data.body.position, Vector3.up * -1.0f);
        Data.xInput = Input.GetAxis("Horizontal");
        Data.yInput = Input.GetAxis("Vertical");

        Data.smoothXInput += (Data.xInput - Data.smoothXInput) * Data.smoothSpeed * Time.deltaTime;
        Data.smoothYInput += (Data.yInput - Data.smoothYInput) * Data.smoothSpeed * Time.deltaTime;

        Data.moveDirLenSq = (Data.xInput * Data.xInput) + (Data.yInput * Data.yInput);

        Data.isGrounded = false;
        RaycastHit hit;
        if (Physics.Raycast(groundRay, out hit, 0.75f))
        {
            if (hit.collider.gameObject.layer != ignoreGroundRay.value)
            {
                Data.isGrounded = true;
            }
        }

        if (Data.animator != null)
        {
            Data.animator.SetBool("IsGrounded", Data.isGrounded);
        }
        //Data.body.useGravity = !Data.isGrounded;

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
        onControlableBreakFree?.Invoke(this);
        Data.body.useGravity = true;
        Destroy(this);
    }

    private void ProcessBreakFree()
    {
        if (Data.prevControlable != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (TryGetComponent<Player>(out _))
                {
                    return;
                }
                Controlable newControlable = Data.prevControlable.AddComponent<Controlable>();
                newControlable.DataSo = DataSo;
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

    public bool CanMove(Vector3 moveDir)
    { 
        if (CheckTerrain(moveDir) == false)
        {
            return false;
        }
        if (CheckObjects(moveDir) == false)
        {
            return false;
        }
        return true;
    }

    private bool CheckObjects(Vector3 moveDir)
    {
        Ray downRay = new Ray(Data.body.position, Vector3.up * -1.0f);
        RaycastHit hit;
        Physics.Raycast(downRay, out hit);

        Ray nextDownRay = new Ray(Data.body.position - Vector3.up * 0.5f, moveDir.normalized);
        RaycastHit nextHit;
        Physics.Raycast(nextDownRay, out nextHit);

        Vector3 normal = hit.normal;
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = hit.point.y;
        float nextHeight = nextHit.point.y;

        if ((angle > maxAngleMovement) && (nextHeight > currentHeight))
        {
            return false;
        }
        return true;
    }

    private bool CheckTerrain(Vector3 moveDir)
    {
        if (Terrain.activeTerrain == null)
        {
            return true;
        }

        Terrain terrain = Terrain.activeTerrain;
        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);
        float currentHeight = terrain.SampleHeight(Data.body.position);
        float nextHeight = terrain.SampleHeight(Data.body.position + moveDir * 5);
        if ((angle > maxAngleMovement) && (nextHeight > currentHeight))
        {
            return false;
        }
        return true;
    }

    private Vector3 GetMapPos()
    {
        Vector3 pos = Data.body.position;
        Terrain terrain = Terrain.activeTerrain;
        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }

    private void OnPauseEnter()
    { 
        isPause = true;
    }

    private void OnPauseExit()
    { 
        isPause = false;
    }

    private void OnEndStateEnter(bool isWinner)
    {
        isPause = true;
    }

    private void OnEndStateExit()
    {
        isPause = false;
    }
}
