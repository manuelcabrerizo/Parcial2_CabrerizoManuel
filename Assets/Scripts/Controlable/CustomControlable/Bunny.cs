using System.Collections.Generic;
using UnityEngine;

public class Bunny : CustomControlable
{
    public override void Initialize(Controlable controlable)
    {
        ControlableData data = controlable.Data;

        State<Controlable> idleState = new ControlableIdleState(controlable, () => { return data.isGrounded && data.moveDirLenSq <= 0.01f; });
        State<Controlable> fowardWalkState = new ControlableFowardWalkState(controlable, () => { return data.isGrounded && data.moveDirLenSq > 0.01f; });
        State<Controlable> highJumpState = new ControlableHighJumpState(controlable, () => { return data.isGrounded && Input.GetKeyDown(KeyCode.Space); });
        State<Controlable> fallState = new ControlableFallState(controlable, () => { return !data.isGrounded && data.body.velocity.y <= 0.0f; });

        StateGraph<Controlable> stateGraph = controlable.StateGraph;
        stateGraph.AddStateTransitions(idleState, new List<State<Controlable>> { fowardWalkState, fallState, highJumpState });
        stateGraph.AddStateTransitions(fowardWalkState, new List<State<Controlable>> { idleState, fallState, highJumpState });
        stateGraph.AddStateTransitions(fallState, new List<State<Controlable>> { idleState, fowardWalkState });
        stateGraph.AddStateTransitions(highJumpState, new List<State<Controlable>> { fallState });

        List<State<Controlable>> basicStates = new List<State<Controlable>> { idleState, fowardWalkState, highJumpState, fallState };
        List<State<Controlable>> additiveStates = new List<State<Controlable>> { };

        stateGraph.AddBasicStates(basicStates);
        stateGraph.AddAdditiveStates(additiveStates);
        stateGraph.SetInitialState(idleState);
    }
}
