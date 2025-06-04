using UnityEngine;

public class Bigfoot : Enemy
{
    [SerializeField] private float attackRadio = 4.0f;
    [SerializeField] private Transform target;

    private BigfootIdleState idleState;
    private BigfootAttackState attackState;
    private Animator animator;

    public float AttackRadio => attackRadio;
    public Transform Target => target;


    private StateMachine stateMachine = new StateMachine();

    private void Awake()
    {
        idleState = GetComponent<BigfootIdleState>();
        attackState = GetComponent<BigfootAttackState>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        attackState.SetBigfoot(this);
        stateMachine.PushState(idleState);
    }

    private void OnDestroy()
    {
        stateMachine.Clear();
    }

    private void Update()
    {
        float distance = (target.position - transform.position).magnitude;
        if (stateMachine.PeekState() as MonoBehaviourState == idleState)
        {
            if (distance <= attackRadio)
            {
                animator.SetBool("IsAttaking", true);
                stateMachine.ChangeState(attackState);
            }
        }
        else if (stateMachine.PeekState() as MonoBehaviourState == attackState)
        {
            if (distance > attackRadio)
            {
                animator.SetBool("IsAttaking", false);
                stateMachine.ChangeState(idleState);
            }
        }
        stateMachine.Update();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadio);
    }
#endif

}
