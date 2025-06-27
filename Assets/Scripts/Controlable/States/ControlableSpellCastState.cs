using System;
using UnityEngine;

public class ControlableSpellCastState : ControlableState
{
    private Player player = null;

    private int controlableLayer;
    private int enemyLayer;
    private int crateProjectileLayer;
    private int movingPlatformLayer;

    public ControlableSpellCastState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition)
    {
        player = controlable.GetComponent<Player>();
        controlableLayer = LayerMask.NameToLayer("Controlable");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        crateProjectileLayer = LayerMask.NameToLayer("Crate Projectile");
        movingPlatformLayer = LayerMask.NameToLayer("MovingPlatform");
        controlable.Data.mousePosX = Screen.width / 2;
        controlable.Data.mousePosY = Screen.height / 2;
    }

    public override void OnEnter()
    {
        ControlableData data = controlable.Data;
        Ray aimRay = data.cam.ScreenPointToRay(new Vector2(data.mousePosX, data.mousePosY));
        player.AimParticleSystem.transform.position = GetAimPosition(data, aimRay);
        player.AimParticleSystem.Play();
        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", true);
        }
    }

    public override void OnExit()
    {
        ProcessSpellCasting();

        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", false);
        }
        player.AimParticleSystem.Stop();
        player.AimParticleSystem.Clear();
    }

    public override void OnUpdate()
    {
        ProcessAiming();
    }

    private Vector3 GetAimPosition(ControlableData data, Ray aimRay)
    {
        Vector3 planePosition = player.transform.position + player.transform.forward;
        Vector3 planeNormal = -player.transform.forward;
        Plane aimingPlane = new Plane(planeNormal, planePosition);
        float t = 0.0f;
        aimingPlane.Raycast(aimRay, out t);
        return aimRay.origin + aimRay.direction * t;
    }

    private void ProcessAiming()
    {
        ControlableData data = controlable.Data;

        // Change Color
        Ray aimRay = data.cam.ScreenPointToRay(new Vector2(data.mousePosX, data.mousePosY));
        RaycastHit hit;
        if (Physics.Raycast(aimRay, out hit))
        {
            GameObject go = hit.collider.gameObject;
            LayerMask layer = go.layer;
            if (go != data.body.gameObject)
            {
                if (layer.value == controlableLayer)
                {
                    player.ParticleRenderer.material = player.ControlMaterial;
                }
                else if (layer.value == enemyLayer)
                {
                    player.ParticleRenderer.material = player.AttackMaterial;
                }
                else if (layer.value == crateProjectileLayer)
                {
                    player.ParticleRenderer.material = player.ControlMaterial;
                }
                else if (layer.value == movingPlatformLayer)
                {
                    player.ParticleRenderer.material = player.ControlMaterial;
                }
                else
                {
                    player.ParticleRenderer.material = player.IdleMaterial;
                }
            }
            else
            {
                player.ParticleRenderer.material = player.IdleMaterial;
            }
        }
        player.AimParticleSystem.transform.position = GetAimPosition(data, aimRay);
    }



    private void ProcessSpellCasting()
    {
        ControlableData data = controlable.Data;

        // Cast Spell
        Ray aimRay = data.cam.ScreenPointToRay(new Vector2(data.mousePosX, data.mousePosY));
        RaycastHit hit;
        if (Physics.Raycast(aimRay, out hit))
        {
            GameObject go = hit.collider.gameObject;
            LayerMask layer = go.layer;
            if (go != data.body.gameObject)
            {
                if (layer.value == controlableLayer)
                {
                    player.SpellParticleRenderer.material = player.ControlMaterial;
                    player.SpellParticleSystem.transform.position = go.transform.position;
                    player.SpellParticleSystem.transform.position += Vector3.up;
                    player.SpellParticleSystem.Play();
                    Controlable newControlable = go.AddComponent<Controlable>();
                    newControlable.SetPrevControlable(controlable.gameObject);
                    controlable.BreakFree();
                }
                else if (layer.value == enemyLayer)
                {
                    Enemy enemy = go.GetComponent<Enemy>();
                    player.SpellParticleRenderer.material = player.AttackMaterial;
                    player.SpellParticleSystem.transform.position = enemy.transform.position;
                    player.SpellParticleSystem.transform.position += Vector3.up * 1.0f;
                    player.SpellParticleSystem.Play();
                    enemy.Attack();
                }
                else if (layer.value == crateProjectileLayer)
                {
                    CrateProjectile crate = go.GetComponent<CrateProjectile>();
                    player.SpellParticleRenderer.material = player.ControlMaterial;
                    player.SpellParticleSystem.transform.position = crate.transform.position;
                    player.SpellParticleSystem.Play();
                    crate.Lunch(crate.transform.position, crate.LaunchPosition, 1.0f);
                }
                else if (layer.value == movingPlatformLayer)
                { 
                    MovingPlatform platform = go.GetComponent<MovingPlatform>();
                    player.SpellParticleRenderer.material = player.ControlMaterial;
                    player.SpellParticleSystem.transform.position = platform.transform.position;
                    player.SpellParticleSystem.Play();
                    platform.MoveFrom(controlable.transform.position);
                }

            }
        }
    }

}
