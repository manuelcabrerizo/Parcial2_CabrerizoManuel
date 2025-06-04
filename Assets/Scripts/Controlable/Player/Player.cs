using UnityEngine;

public class Player : Controlable
{
    [SerializeField] private ParticleSystem aimParticleSystem;
    private PlayerAim playerAim;

    protected override void OnAwaken()
    {
        base.OnAwaken();
        playerAim = GetComponent<PlayerAim>();
    }

    private void Start()
    {
        Control(null);
    }

    protected override void OnControlEnter()
    {
        aimParticleSystem.Play();
        playerAim.enabled = true;
        base.OnControlEnter();
    }

    protected override void OnControlExit()
    {
        aimParticleSystem.Stop();
        playerAim.enabled = false;
        base.OnControlExit();
    }
}