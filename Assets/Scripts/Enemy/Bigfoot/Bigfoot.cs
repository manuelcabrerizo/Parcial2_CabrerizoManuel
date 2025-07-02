using System;
using System.Collections.Generic;
using UnityEngine;

public class Bigfoot : Enemy
{
    public static event Action<Bigfoot> onSpawnCrate;
    public static event Action<Bigfoot> onLunchCrate;

    [SerializeField] private Transform hand = null;
    [SerializeField] private float attackRadio = 4.0f;

    private StateGraph<Bigfoot> stateGraph;
    public Transform Hand => hand;
    public Animator Animator { get; private set; }
    public Rigidbody Body { get; private set; }

    public float AttackRadio => attackRadio;
    public float Distance { get; private set; }

    public PatrolPoints PatrolPoints { get; private set; }

    protected override void OnAwaken()
    {
        Animator = GetComponent<Animator>();
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

    private void InitializeStates()
    {
        State<Bigfoot> idleState = new BigfootIdleState(this,
            () => { return Distance > AttackRadio && PatrolPoints.GetCurrent() == null; },
            () => { return Distance <= AttackRadio; });

        State<Bigfoot> patrolState = new BigfootPatrolState(this,
            () => { return Distance > AttackRadio && PatrolPoints.GetCurrent(); },
            () => { return Distance <= AttackRadio; });

        State<Bigfoot> attackState = new BigfootAttackState(this,
            () => { return Distance <= AttackRadio; },
            () => { return Distance > AttackRadio; });

        stateGraph.AddStateTransitions(idleState, new List<State<Bigfoot>> { patrolState, attackState });
        stateGraph.AddStateTransitions(attackState, new List<State<Bigfoot>> { idleState, patrolState });
        stateGraph.AddStateTransitions(patrolState, new List<State<Bigfoot>> { idleState, attackState });


        List<State<Bigfoot>> basicStates = new List<State<Bigfoot>> { idleState, attackState, patrolState };
        List<State<Bigfoot>> additiveStates = new List<State<Bigfoot>> { };

        stateGraph.AddBasicStates(basicStates);
        stateGraph.AddAdditiveStates(additiveStates);
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

    // Methods call from the animator
    public void SpawnCrate()
    {
        onSpawnCrate?.Invoke(this);
    }

    public void LunchCrate()
    {
        onLunchCrate?.Invoke(this);
    }
}
