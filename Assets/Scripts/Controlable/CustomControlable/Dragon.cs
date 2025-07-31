using System.Collections.Generic;
using UnityEngine;

public class Dragon : CustomControlable
{
    private void Awake()
    {
        Controlable.onControlableBreakFree += OnControlableBreakFree;
    }

    private void OnDestroy()
    {
        Controlable.onControlableBreakFree -= OnControlableBreakFree;
    }

    public override void Initialize(Controlable controlable)
    {
        ControlableData data = controlable.Data;

        State<Controlable> idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        State<Controlable> walkState = new ControlableFowardWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        State<Controlable> flyState = new ControlableFlyState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        State<Controlable> fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });

        StateGraph<Controlable> stateGraph = controlable.StateGraph;
        stateGraph.AddStateTransitions(idleState, new List<State<Controlable>> { walkState, flyState, fallState });
        stateGraph.AddStateTransitions(walkState, new List<State<Controlable>> { idleState, flyState, fallState });
        stateGraph.AddStateTransitions(fallState, new List<State<Controlable>> { idleState, walkState });
        stateGraph.AddStateTransitions(flyState, new List<State<Controlable>> { idleState, walkState });

        List<State<Controlable>> basicStates = new List<State<Controlable>> { idleState, walkState, fallState, flyState };
        List<State<Controlable>> additiveStates = new List<State<Controlable>> { };

        stateGraph.AddBasicStates(basicStates);
        stateGraph.AddAdditiveStates(additiveStates);
        stateGraph.SetInitialState(idleState);
    }

    private void OnControlableBreakFree(Controlable controlable)
    {
        if (controlable.TryGetComponent<Dragon>(out _))
        {
            controlable.Data.animator.SetBool("IsGrounded", true);
            controlable.Data.animator.SetFloat("Velocity", 0.0f);
        }
    }
}
