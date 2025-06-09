using UnityEngine;

public class Bigfoot : Enemy
{
    [SerializeField] private Transform hand = null;
    [SerializeField] private Transform target = null;
    [SerializeField] private float attackRadio = 4.0f;
    private StateMachine stateMachine = null;
    private BigfootIdleState idleState = null;
    private BigfootAttackState attackState = null;
    private Animator animator;

    public Transform Hand => hand;
    public Transform Target => target;
    public float AttackRadio => attackRadio;
    public StateMachine StateMachine => stateMachine;
    public BigfootIdleState IdleState => idleState;
    public BigfootAttackState AttackState => attackState;
    public Animator Animator => animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine = new StateMachine();
        idleState = new BigfootIdleState(this);
        attackState = new BigfootAttackState(this);
    }

    private void Start()
    {
        stateMachine.PushState(idleState);
    }

    private void OnDestroy()
    {
        stateMachine.Clear();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    // Methods call from the animator
    public void SpawnCrate()
    {
        attackState.SpawnCrate();
    }

    public void LunchCrate()
    {
        attackState.LunchCrate();
    }

}
