using UnityEngine;

public class ControlableSpellCastState : ControlableState
{
    private float mousePosX = 0.0f;
    private float mousePosY = 0.0f;
    private ParticleSystemRenderer particleRenderer = null;
    private ParticleSystemRenderer spellParticleRenderer = null;

    int controlableLayer;
    int enemyLayer;
    int crateProjectileLayer;

    public ControlableSpellCastState(Controlable controlable) 
        : base(controlable)
    {
        controlableLayer = LayerMask.NameToLayer("Controlable");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        crateProjectileLayer = LayerMask.NameToLayer("Crate Projectile");
    }

    public override void OnEnter()
    {
        if(particleRenderer == null)
        {
            particleRenderer = Controlable.player.AimParticleSystem.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = Controlable.player.IdleMaterial;
        }
        if (spellParticleRenderer == null)
        {
            spellParticleRenderer = Controlable.player.SpellParticleSystem.GetComponent<ParticleSystemRenderer>();
        }

        ControlableData data = controlable.Data;
        mousePosX = Screen.width / 2;
        mousePosY = Screen.height / 2;
        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", true);
        }
        Controlable.player.AimParticleSystem.Clear();
        Controlable.player.AimParticleSystem.Play();
    }

    public override void OnExit()
    {
        ControlableData data = controlable.Data;
        Controlable.player.AimParticleSystem.Stop();
        if (data.animator != null)
        {
            data.animator.SetBool("IsAiming", false);
        }
    }

    public override void OnUpdate()
    {
        ProcessAiming();
        ProcessSpellCasting();
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

        mousePosX = Mathf.Clamp(mousePosX + mouseX * mouseSpeed, 0.0f, Screen.width);
        mousePosY = Mathf.Clamp(mousePosY + mouseY * mouseSpeed, 0.0f, Screen.height);

        Ray ray = data.cam.ScreenPointToRay(new Vector2(mousePosX, mousePosY));
        float t;
        if (aimingPlane.Raycast(ray, out t))
        {
            Controlable.player.AimParticleSystem.transform.position = ray.origin + ray.direction * t;
        }

        Vector3 screenPoint = data.cam.WorldToScreenPoint(Controlable.player.AimParticleSystem.transform.position);
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
                    particleRenderer.material = Controlable.player.ControlMaterial;
                }
                else if (layer.value == enemyLayer)
                {
                    particleRenderer.material = Controlable.player.AttackMaterial;
                }
                else if (layer.value == crateProjectileLayer)
                {
                    particleRenderer.material = Controlable.player.ControlMaterial;
                }
                else
                {
                    particleRenderer.material = Controlable.player.IdleMaterial;
                }
            }
        }
    }

    private void ProcessSpellCasting()
    {
        ControlableData data = controlable.Data;

        if (Input.GetMouseButtonUp(0))
        {
            Vector3 screenPoint = data.cam.WorldToScreenPoint(Controlable.player.AimParticleSystem.transform.position);
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
                        
                        spellParticleRenderer.material = Controlable.player.ControlMaterial;
                        Controlable.player.SpellParticleSystem.transform.position = go.transform.position;
                        Controlable.player.SpellParticleSystem.transform.position += Vector3.up;
                        Controlable.player.SpellParticleSystem.Play();

                        Controlable newControlable = go.AddComponent<Controlable>();
                        newControlable.SetType(ControlableType.Object);
                        controlable.BreakFree();
                        
                    }
                    else if (layer.value == enemyLayer)
                    {
                        
                        Enemy enemy = go.GetComponent<Enemy>();
                        spellParticleRenderer.material = Controlable.player.AttackMaterial;
                        Controlable.player.SpellParticleSystem.transform.position = enemy.transform.position;
                        Controlable.player.SpellParticleSystem.transform.position += Vector3.up * 1.0f;
                        Controlable.player.SpellParticleSystem.Play();
                        enemy.Attack();
                        
                    }
                    else if (layer.value == crateProjectileLayer)
                    {
                        CrateProjectile crate = go.GetComponent<CrateProjectile>();
                        spellParticleRenderer.material = Controlable.player.IdleMaterial;
                        Controlable.player.SpellParticleSystem.transform.position = crate.transform.position;
                        Controlable.player.SpellParticleSystem.Play();
                        crate.Lunch(crate.transform.position, crate.LaunchPosition, 1.0f);
                    }

                }
            }

        }
    }

}
