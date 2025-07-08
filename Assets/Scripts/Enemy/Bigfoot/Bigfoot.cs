using System;
using System.Collections.Generic;
using UnityEngine;

public class Bigfoot : Enemy
{
    public static event Action<Bigfoot> onSpawnCrate;
    public static event Action<Bigfoot> onLunchCrate;

    [SerializeField] private LayerMask damageMask;
    [SerializeField] private Transform hand = null;
    [field: SerializeField] public Transform Aim { get; private set; } 

    [SerializeField] private float attackRadio = 4.0f;
    [field:SerializeField] public SoundClipsSO Clips { get; private set; }

    [field: SerializeField] public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }

    private StateGraph<Bigfoot> stateGraph;

    private bool isHit = false;
    private bool isDead = false;
    private bool isDeadAnimationEnd = false;

    public Transform Hand => hand;
    public Animator Animator { get; private set; }
    public Rigidbody Body { get; private set; }

    public Collider Collision { get; private set; }

    public float AttackRadio => attackRadio;
    public float Distance { get; private set; }

    public PatrolPoints PatrolPoints { get; private set; }

    public bool IsDead => isDead;
    public bool IsDeadAnimationEnd => isDeadAnimationEnd;

    protected override void OnAwaken()
    {
        Animator = GetComponent<Animator>();
        Collision = GetComponent<Collider>();
        PatrolPoints = GetComponent<PatrolPoints>();
        Body = GetComponent<Rigidbody>();
        stateGraph = new StateGraph<Bigfoot>();
    }

    protected override void OnStart()
    {
        InitializeStates();
    }

    protected override void OnDestroyed()
    {
        SkinnedMeshRenderer.material.SetColor("_Tint", Color.black);
        stateGraph.Clear();
    }

    private void Update()
    {
        if (Target == null)
        {
            return;
        }

        ProcessData();
        stateGraph.Update();
    }

    private void FixedUpdate()
    {
        stateGraph.FixedUpdate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Utils.CheckCollisionLayer(collision.gameObject, damageMask))
        {
            CrateProjectile crate = collision.gameObject.GetComponent<CrateProjectile>();
            if (crate.CanMakeDamage())
            {
                TakeDamage(2);
            }
        }
    }

    private void InitializeStates()
    {
        State<Bigfoot> idleState = new BigfootIdleState(this,
            () => { return Distance > AttackRadio && !isHit && !isDead && PatrolPoints.GetCurrent() == null; },
            () => { return Distance <= AttackRadio || isHit || isDead; });

        State<Bigfoot> patrolState = new BigfootPatrolState(this,
            () => { return Distance > AttackRadio && !isHit && !isDead && PatrolPoints.GetCurrent(); },
            () => { return Distance <= AttackRadio || isHit || isDead; });

        State<Bigfoot> attackState = new BigfootAttackState(this,
            () => { return Distance <= AttackRadio && !isHit && !isDead; },
            () => { return Distance > AttackRadio || isHit || isDead; });

        State<Bigfoot> hitState = new BigfootHitState(this,
            () => { return isHit; });

        State<Bigfoot> deadState = new BigfootDeadState(this,
            () => { return isDead; });

        stateGraph.AddStateTransitions(idleState, new List<State<Bigfoot>> { patrolState, attackState, hitState, deadState });
        stateGraph.AddStateTransitions(attackState, new List<State<Bigfoot>> { idleState, patrolState, hitState, deadState });
        stateGraph.AddStateTransitions(patrolState, new List<State<Bigfoot>> { idleState, attackState, hitState, deadState });
        stateGraph.AddStateTransitions(hitState, new List<State<Bigfoot>> { idleState, patrolState, attackState });
        stateGraph.AddStateTransitions(deadState, new List<State<Bigfoot>> { });
        List<State<Bigfoot>> basicStates = new List<State<Bigfoot>> { idleState, attackState, patrolState, hitState, deadState };

        stateGraph.AddBasicStates(basicStates);
        stateGraph.AddAdditiveStates(new List<State<Bigfoot>> { });

        if (PatrolPoints.GetCurrent())
        {
            stateGraph.SetInitialState(patrolState);
        }
        else
        {
            stateGraph.SetInitialState(idleState);
        }
    }

    private void ProcessData()
    {
        Distance = (Target.position - transform.position).magnitude;
    }

    public override void TakeDamage(int amount)
    {
        if (!isHit)
        {
            base.TakeDamage(amount);
            if (life > 0)
            {
                isHit = true;
            }
            else
            {
                isDead = true;
            }
        }
    }

    public void SetIsHit(bool value)
    { 
        isHit = value;
    }

    // Methods call from the animator
    public void SpawnCrate()
    {
        onSpawnCrate?.Invoke(this);
    }

    public void LunchCrate()
    {
        onLunchCrate?.Invoke(this);
    }

    public void StartRoar()
    {
        AudioManager.onPlayClip3D?.Invoke(Clips.monsterAttack, transform.position, 100, 400);
    }

    public void Kill()
    {
        Body.isKinematic = true;
        Collision.enabled = false;
        Animator.enabled = false;
        isDeadAnimationEnd = true;
        SkinnedMeshRenderer.material.SetColor("_Tint", Color.black);
    }

    public void HitAnimationEnd()
    {
        isHit = false;
    }

    public void MakeStepSound()
    {
        AudioManager.onPlayClip3D?.Invoke(Clips.monsterStep, transform.position, 100, 400);
    }
}
