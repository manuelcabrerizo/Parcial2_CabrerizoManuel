using System;
using System.Collections;
using UnityEngine;

public class EndState : State<GameManager>
{
    public static event Action<bool> onEndStateEnter;
    public static event Action onEndStateExit;
    public EndState(GameManager owner)
        : base(owner) { }

    public override void OnEnter()
    {
        onEndStateEnter?.Invoke(owner.IsWinner);
        owner.StartCoroutine(WaitSeconds(10.0f));
    }

    public override void OnExit()
    {
        onEndStateExit?.Invoke();
    }

    IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (owner.IsWinner)
        {
            owner.SetWinState();
        }
        else
        {
            owner.SetGameOverState();
        }
    }
}
