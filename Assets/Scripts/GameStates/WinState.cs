using System;
using UnityEngine;

public class WinState : State<GameManager>
{
    public static event Action onWinStateEnter;
    public static event Action onWinStateExit;

    public WinState(GameManager owner) 
        : base(owner) { }

    public override void OnEnter()
    {
        Cursor.lockState = CursorLockMode.None;
        onWinStateEnter?.Invoke();
        Time.timeScale = 0.0f;
    }

    public override void OnExit()
    {
        onWinStateExit?.Invoke();
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
