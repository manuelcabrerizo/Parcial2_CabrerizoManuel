using System;
using UnityEngine;

public class ControlableSpellCastState : ControlableState
{
    private float mousePosX = 0.0f;
    private float mousePosY = 0.0f;
    private ParticleSystemRenderer particleRenderer = null;
    private ParticleSystemRenderer spellParticleRenderer = null;

    private int controlableLayer;
    private int enemyLayer;
    private int crateProjectileLayer;

    public ControlableSpellCastState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition)
    {
        controlableLayer = LayerMask.NameToLayer("Controlable");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        crateProjectileLayer = LayerMask.NameToLayer("Crate Projectile");
        mousePosX = Screen.width / 2;
        mousePosY = Screen.height / 2;
    }

    public override void OnEnter()
    {
        ControlableData data = controlable.Data;

        if (particleRenderer == null)
        {
            particleRenderer = data.player.AimParticleSystem.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = data.player.IdleMaterial;
        }
        if (spellParticleRenderer == null)
        {
            spellParticleRenderer = data.player.SpellParticleSystem.GetComponent<ParticleSystemRenderer>();
        }

        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", true);
        }
        data.player.AimParticleSystem.Play();
    }

    public override void OnExit()
    {
        ProcessSpellCasting();

        ControlableData data = controlable.Data;
        data.player.AimParticleSystem.Clear();
        data.player.AimParticleSystem.Stop();
        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", false);
        }
    }

    public override void OnUpdate()
    {
        ProcessAiming();
    }

    private void ProcessAiming()
    {
        ControlableData data = controlable.Data;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float mouseSpeed = 8.0f;

        Vector3 planePosition = data.cameraMovement.transform.position + data.cameraMovement.transform.forward * 4;
        Vector3 planeNormal = -data.cameraMovement.transform.forward;
        Plane aimingPlane = new Plane(planeNormal, planePosition);

        mousePosX += mouseX * mouseSpeed;
        mousePosY += mouseY * mouseSpeed;
        float radio = Screen.height * 0.4f;
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Vector2 mousePos = new Vector2(mousePosX, mousePosY);
        if ((mousePos - center).sqrMagnitude > radio * radio)
        {
            mousePos = center + (mousePos - center).normalized * radio;
        }
        mousePosX = mousePos.x;
        mousePosY = mousePos.y;

        Ray ray = data.cam.ScreenPointToRay(new Vector2(mousePosX, mousePosY));
        float t;
        if (aimingPlane.Raycast(ray, out t))
        {
            data.player.AimParticleSystem.transform.position = ray.origin + ray.direction * t;
        }

        Vector3 screenPoint = data.cam.WorldToScreenPoint(data.player.AimParticleSystem.transform.position);
        Ray aimRay = data.cam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(aimRay, out hit))
        {
            GameObject go = hit.collider.gameObject;
            LayerMask layer = go.layer;
            if (go != data.body.gameObject)
            {
                if (layer.value == controlableLayer)
                {
                    particleRenderer.material = data.player.ControlMaterial;
                }
                else if (layer.value == enemyLayer)
                {
                    particleRenderer.material = data.player.AttackMaterial;
                }
                else if (layer.value == crateProjectileLayer)
                {
                    particleRenderer.material = data.player.ControlMaterial;
                }
                else
                {
                    particleRenderer.material = data.player.IdleMaterial;
                }
            }
        }
        else
        {
            particleRenderer.material = data.player.IdleMaterial;
        }
    }

    private bool IsType<Type>(GameObject go) where Type : MonoBehaviour
    {
        Type component = null;
        go.TryGetComponent<Type>(out component);
        return component != null;
    }

    private void ProcessSpellCasting()
    {
        ControlableData data = controlable.Data;

        Vector3 screenPoint = data.cam.WorldToScreenPoint(data.player.AimParticleSystem.transform.position);
        Ray aimRay = data.cam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(aimRay, out hit))
        {
            GameObject go = hit.collider.gameObject;
            LayerMask layer = go.layer;
            if (go != data.body.gameObject)
            {
                if (layer.value == controlableLayer)
                {       
                    spellParticleRenderer.material = data.player.ControlMaterial;
                    data.player.SpellParticleSystem.transform.position = go.transform.position;
                    data.player.SpellParticleSystem.transform.position += Vector3.up;
                    data.player.SpellParticleSystem.Play();

                    Controlable newControlable = go.AddComponent<Controlable>();
                    if (IsType<Player>(go))
                    {
                        Player.InitControlable(newControlable);
                    }
                    else if (IsType<Bunny>(go))
                    {
                        Bunny.InitControlable(newControlable);
                    }
                    else if (IsType<Dragon>(go))
                    {
                        Dragon.InitControlable(newControlable);
                    }
                    else
                    {
                        Object.InitControlable(newControlable);
                    }
                    newControlable.SetPlayer(data.player);
                    controlable.BreakFree(); 
                }
                else if (layer.value == enemyLayer)
                {
                        
                    Enemy enemy = go.GetComponent<Enemy>();
                    spellParticleRenderer.material = data.player.AttackMaterial;
                    data.player.SpellParticleSystem.transform.position = enemy.transform.position;
                    data.player.SpellParticleSystem.transform.position += Vector3.up * 1.0f;
                    data.player.SpellParticleSystem.Play();
                    enemy.Attack();
                        
                }
                else if (layer.value == crateProjectileLayer)
                {
                    CrateProjectile crate = go.GetComponent<CrateProjectile>();
                    spellParticleRenderer.material = data.player.IdleMaterial;
                    data.player.SpellParticleSystem.transform.position = crate.transform.position;
                    data.player.SpellParticleSystem.Play();
                    crate.Lunch(crate.transform.position, crate.LaunchPosition, 1.0f);
                }

            }
        }
    }

}
