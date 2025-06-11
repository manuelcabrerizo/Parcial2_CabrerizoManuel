using System.Collections.Generic;
using UnityEngine;

public class Bunny : MonoBehaviour
{
    static public void InitControlable(Controlable controlable)
    {
        ControlableData data = controlable.Data;
        
        ControlableState idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        ControlableState fowardWalkState = new ControlableFowardWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        ControlableState highJumpState = new ControlableHighJumpState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        ControlableState fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });

        StateGraph stateGraph = new StateGraph();
        stateGraph.AddStateTransitions(idleState, new List<State> { fowardWalkState, fallState, highJumpState });
        stateGraph.AddStateTransitions(fowardWalkState, new List<State> { idleState, fallState, highJumpState });
        stateGraph.AddStateTransitions(fallState, new List<State> { idleState, fowardWalkState });
        stateGraph.AddStateTransitions(highJumpState, new List<State> { fallState });

        List<ControlableState> basicStates = new List<ControlableState> { idleState, fowardWalkState, highJumpState, fallState };
        List<ControlableState> additiveStates = new List<ControlableState> { };

        controlable.SetStates(basicStates, additiveStates, stateGraph, idleState);
    }
}
