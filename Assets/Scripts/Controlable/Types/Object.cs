using System.Collections.Generic;
using UnityEngine;

public class Object
{
    static public void InitControlable(Controlable controlable)
    {
        ControlableData data = controlable.Data;

        ControlableState idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        ControlableState walkState = new ControlableWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        ControlableState jumpState = new ControlableJumpState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });

        StateGraph stateGraph = new StateGraph();
        stateGraph.AddStateTransitions(idleState, new List<State> { walkState, fallState, jumpState });
        stateGraph.AddStateTransitions(walkState, new List<State> { idleState, fallState, jumpState });
        stateGraph.AddStateTransitions(fallState, new List<State> { idleState, walkState });
        stateGraph.AddStateTransitions(jumpState, new List<State> { fallState });

        List<ControlableState> basicStates = new List<ControlableState> { idleState, walkState, fallState, jumpState };
        List<ControlableState> additiveStates = new List<ControlableState> { };

        controlable.SetStates(basicStates, additiveStates, stateGraph, idleState);
    }
}
