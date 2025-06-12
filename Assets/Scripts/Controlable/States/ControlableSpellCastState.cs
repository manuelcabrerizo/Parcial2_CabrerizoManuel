using System;
using UnityEngine;

public class ControlableSpellCastState : ControlableState
{
    private Player player = null;

    private float mousePosX = 0.0f;
    private float mousePosY = 0.0f;

    private int controlableLayer;
    private int enemyLayer;
    private int crateProjectileLayer;

    public ControlableSpellCastState(Controlable controlable, Func<bool> condition) 
        : base(controlable, condition)
    {
        player = controlable.GetComponent<Player>();
        controlableLayer = LayerMask.NameToLayer("Controlable");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        crateProjectileLayer = LayerMask.NameToLayer("Crate Projectile");
        mousePosX = Screen.width / 2;
        mousePosY = Screen.height / 2;
    }

    public override void OnEnter()
    {
        ControlableData data = controlable.Data;
        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", true);
        }
        player.AimParticleSystem.Play();
    }

    public override void OnExit()
    {
        ProcessSpellCasting();

        ControlableData data = controlable.Data;
        player.AimParticleSystem.Clear();
        player.AimParticleSystem.Stop();
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
            player.AimParticleSystem.transform.position = ray.origin + ray.direction * t;
        }

        Vector3 screenPoint = data.cam.WorldToScreenPoint(player.AimParticleSystem.transform.position);
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
                else
                {
                    player.ParticleRenderer.material = player.IdleMaterial;
                }
            }
        }
        else
        {
            player.ParticleRenderer.material = player.IdleMaterial;
        }
    }



    private void ProcessSpellCasting()
    {
        ControlableData data = controlable.Data;

        Vector3 screenPoint = data.cam.WorldToScreenPoint(player.AimParticleSystem.transform.position);
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
                    player.SpellParticleRenderer.material = player.ControlMaterial;
                    player.SpellParticleSystem.transform.position = go.transform.position;
                    player.SpellParticleSystem.transform.position += Vector3.up;
                    player.SpellParticleSystem.Play();
                    Controlable newControlable = go.AddComponent<Controlable>();
                    newControlable.Initialize();
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
                    player.SpellParticleRenderer.material = player.IdleMaterial;
                    player.SpellParticleSystem.transform.position = crate.transform.position;
                    player.SpellParticleSystem.Play();
                    crate.Lunch(crate.transform.position, crate.LaunchPosition, 1.0f);
                }

            }
        }
    }

}
