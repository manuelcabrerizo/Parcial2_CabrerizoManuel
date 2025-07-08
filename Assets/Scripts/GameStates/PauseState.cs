using System;
using UnityEngine;

public class PauseState : State<GameManager>
{
    public static event Action onPauseStateEnter;
    public static event Action onPauseStateExit;
    public PauseState(GameManager owner) 
        : base(owner) { }


    public override void OnEnter()
    {
        Cursor.lockState = CursorLockMode.None;
        onPauseStateEnter?.Invoke();
        Time.timeScale = 0.0f;
    }

    public override void OnExit()
    {
        onPauseStateExit?.Invoke();
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
