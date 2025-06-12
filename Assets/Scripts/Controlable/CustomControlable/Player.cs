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
        ParticleRenderer = AimParticleSystem.GetComponent<ParticleSystemRenderer>();
        SpellParticleRenderer = SpellParticleSystem.GetComponent<ParticleSystemRenderer>();
        ParticleRenderer.material = IdleMaterial;
    }

    public override void Initialize(Controlable controlable)
    {
        ControlableData data = controlable.Data;
        // Basic states
        ControlableState idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        ControlableState walkState = new ControlableWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        ControlableState jumpState = new ControlableJumpState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });
        // Additive states
        ControlableState spellCastState = new ControlableSpellCastState(controlable, () => { return Input.GetMouseButton(0); });

        StateGraph stateGraph = new StateGraph();
        stateGraph.AddStateTransitions(idleState, new List<State> { walkState, fallState, jumpState, spellCastState });
        stateGraph.AddStateTransitions(walkState, new List<State> { idleState, fallState, jumpState, spellCastState });
        stateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState, spellCastState });
        stateGraph.AddStateTransitions(jumpState, new List<State> { fallState, spellCastState });
        stateGraph.AddStateTransitions(spellCastState, new List<State> { idleState, walkState, jumpState, fallState });

        List<ControlableState> basicStates = new List<ControlableState> { idleState, walkState, jumpState, fallState };
        List<ControlableState> additiveStates = new List<ControlableState> { spellCastState };

        controlable.SetStates(basicStates, additiveStates, stateGraph, idleState);
    }
}
