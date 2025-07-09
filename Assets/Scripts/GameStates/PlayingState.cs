using System.Collections;
using UnityEngine;

public class PlayingState : State<GameManager>
{
    public PlayingState(GameManager owner)
        : base(owner) { }

    public override void OnEnter()
    {
        Cursor.lockState = CursorLockMode.Locked;
        AudioManager.onPlayMusic?.Invoke();
    }

    public override void OnExit()
    {
        AudioManager.onPauseMusic?.Invoke();
    }
}
