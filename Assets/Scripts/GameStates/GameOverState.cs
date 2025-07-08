using System;
using UnityEngine;

public class GameOverState : State<GameManager>
{
    public static event Action onGameOverStateEnter;
    public static event Action onGameOverStateExit;
    public GameOverState(GameManager owner) 
        : base(owner) { }

    public override void OnEnter()
    {
        Cursor.lockState = CursorLockMode.None;
        onGameOverStateEnter?.Invoke();
        Time.timeScale = 0.0f;
    }

    public override void OnExit() 
    {
        onGameOverStateExit?.Invoke();
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
