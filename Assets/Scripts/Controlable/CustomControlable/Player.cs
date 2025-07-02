using System.Collections.Generic;
using UnityEngine;

public class Player : CustomControlable
{
    [SerializeField] private ParticleSystem aimParticleSystem;
    [SerializeField] private ParticleSystem spellParticleSystem;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material controlMaterial;
    [SerializeField] private Material attackMaterial;
    public ParticleSystem AimParticleSystem => aimParticleSystem;
    public ParticleSystem SpellParticleSystem => spellParticleSystem;
    public Material IdleMaterial => idleMaterial;
    public Material ControlMaterial => controlMaterial;
    public Material AttackMaterial => attackMaterial;
    public ParticleSystemRenderer ParticleRenderer { get; private set; }
    public ParticleSystemRenderer SpellParticleRenderer { get; private set; }

    private void Awake()
    {
        Enemy.onEnemySpawn += OnEnemySpawn;

        ParticleRenderer = AimParticleSystem.GetComponent<ParticleSystemRenderer>();
        SpellParticleRenderer = SpellParticleSystem.GetComponent<ParticleSystemRenderer>();
        ParticleRenderer.material = IdleMaterial;
        // take this game object out of the player, so its position is relative to the world not the player
        spellParticleSystem.gameObject.transform.parent = gameObject.transform.parent;
    }

    private void OnDestroy()
    {
        Enemy.onEnemySpawn -= OnEnemySpawn;
    }

    public override void Initialize(Controlable controlable)
    {
        ControlableData data = controlable.Data;
        // Basic states
        State<Controlable> idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        State<Controlable> walkState = new ControlableWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        State<Controlable> jumpState = new ControlableJumpState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        State<Controlable> fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });
        // Additive states
        State<Controlable> spellCastState = new ControlableSpellCastState(controlable, () => { return Input.GetMouseButton(0); });

        StateGraph<Controlable> stateGraph = controlable.StateGraph;
        stateGraph.AddStateTransitions(idleState, new List<State<Controlable>> { walkState, fallState, jumpState, spellCastState });
        stateGraph.AddStateTransitions(walkState, new List<State<Controlable>> { idleState, fallState, jumpState, spellCastState });
        stateGraph.AddStateTransitions(fallState, new List<State<Controlable>> { idleState, walkState, spellCastState });
        stateGraph.AddStateTransitions(jumpState, new List<State<Controlable>> { fallState, spellCastState });
        stateGraph.AddStateTransitions(spellCastState, new List<State<Controlable>> { idleState, walkState, jumpState, fallState });

        List<State<Controlable>> basicStates = new List<State<Controlable>> { idleState, walkState, jumpState, fallState };
        List<State<Controlable>> additiveStates = new List<State<Controlable>> { spellCastState };

        stateGraph.AddBasicStates(basicStates);
        stateGraph.AddAdditiveStates(additiveStates);
        stateGraph.SetInitialState(idleState);
    }

    private void OnEnemySpawn(Enemy enemy)
    {
        enemy.SetTarget(transform);
    }
}
