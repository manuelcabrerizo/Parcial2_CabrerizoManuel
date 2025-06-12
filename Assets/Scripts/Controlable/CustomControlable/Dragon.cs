using System.Collections.Generic;
using UnityEngine;

public class Dragon : CustomControlable
{
    public override void Initialize(Controlable controlable)
    {
        ControlableData data = controlable.Data;

        ControlableState idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        ControlableState walkState = new ControlableWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        ControlableState flyState = new ControlableFlyState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });

        StateGraph stateGraph = new StateGraph();
        stateGraph.AddStateTransitions(idleState, new List<State> { walkState, flyState, fallState });
        stateGraph.AddStateTransitions(walkState, new List<State> { idleState, flyState, fallState });
        stateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState });
        stateGraph.AddStateTransitions(flyState, new List<State> { idleState, walkState });

        List<ControlableState> basicStates = new List<ControlableState> { idleState, walkState, fallState, flyState };
        List<ControlableState> additiveStates = new List<ControlableState> { };

        controlable.SetStates(basicStates, additiveStates, stateGraph, idleState);
    }
}
