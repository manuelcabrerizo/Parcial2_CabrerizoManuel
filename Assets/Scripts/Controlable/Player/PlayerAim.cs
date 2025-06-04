using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private ParticleSystem aimParticleSystem;
    [SerializeField] private ParticleSystem spellParticleSystem;
    [SerializeField] private Material idleMaterial;
    [SerializeField] private Material controlMaterial;
    [SerializeField] private Material attackMaterial;

    private float mousePosX = 0.0f;
    private float mousePosY = 0.0f;

    private Animator animator;
    private ParticleSystemRenderer particleRenderer;
    private ParticleSystemRenderer spellParticleRenderer;

    private Player player;
    private CameraMovement cameraMovement;
    private Camera cam;

    private void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponent<Animator>();

        CameraMovement.onCameraCreate += OnCameraCreate;

        mousePosX = Screen.width / 2;
        mousePosY = Screen.height / 2;

        particleRenderer = aimParticleSystem.GetComponent<ParticleSystemRenderer>();
        particleRenderer.material = idleMaterial;
        spellParticleRenderer = spellParticleSystem.GetComponent<ParticleSystemRenderer>();
    }

    private void OnDestroy()
    {
        CameraMovement.onCameraCreate -= OnCameraCreate;
    }

    private void Update()
    {
        if (cameraMovement.IsAiming())
        {
            if (aimParticleSystem.isPlaying == false)
            {
                mousePosX = Screen.width / 2;
                mousePosY = Screen.height / 2;
                animator.SetBool("IsAiming", true);
                aimParticleSystem.Play();
            }
        }
        else
        {
            if (aimParticleSystem.isPlaying == true)
            {
                aimParticleSystem.Clear();
                aimParticleSystem.Stop();
                animator.SetBool("IsAiming", false);
            }
        }
        ProcessAiming();
    }

    private void ProcessAiming()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float mouseSpeed = 8.0f;

            Vector3 planePosition = cameraMovement.transform.position + cameraMovement.transform.forward * 4;
            Vector3 planeNormal = -cameraMovement.transform.forward;
            Plane aimingPlane = new Plane(planeNormal, planePosition);

            mousePosX = Mathf.Clamp(mousePosX + mouseX * mouseSpeed, 0.0f, Screen.width);
            mousePosY = Mathf.Clamp(mousePosY + mouseY * mouseSpeed, 0.0f, Screen.height);

            Ray ray = cam.ScreenPointToRay(new Vector2(mousePosX, mousePosY));
            float t;
            if (aimingPlane.Raycast(ray, out t))
            {
                aimParticleSystem.transform.position = ray.origin + ray.direction * t;
            }

            // TODO: use layers for this check
            Vector3 screenPoint = cam.WorldToScreenPoint(aimParticleSystem.transform.position);
            Ray aimRay = cam.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            if (Physics.Raycast(aimRay, out hit))
            {
                GameObject go = hit.collider.gameObject;
                if (go != gameObject)
                {
                    Controlable controlable;
                    Enemy enemy;
                    if (go.TryGetComponent<Controlable>(out controlable))
                    {
                        particleRenderer.material = controlMaterial;
                    }
                    else if (go.TryGetComponent<Enemy>(out enemy))
                    {
                        particleRenderer.material = attackMaterial;
                    }
                    else
                    {
                        particleRenderer.material = idleMaterial;
                    }
                }
            }

        }
        
        // TODO: use layers for this
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(aimParticleSystem.transform.position);
            Ray aimRay = cam.ScreenPointToRay(screenPoint);
            RaycastHit hit;
            if (Physics.Raycast(aimRay, out hit))
            {
                GameObject go = hit.collider.gameObject;
                if (go != gameObject)
                {
                    Controlable controlable;
                    Enemy enemy;
                    if (go.TryGetComponent<Controlable>(out controlable))
                    {
                        spellParticleRenderer.material = controlMaterial;
                        spellParticleSystem.transform.position = controlable.transform.position;
                        spellParticleSystem.transform.position += Vector3.up;
                        spellParticleSystem.Play();
                        Destroy(GetComponent<EntityMovement>());
                        controlable.Control(player);
                    }
                    else if (go.TryGetComponent<Enemy>(out enemy))
                    {
                        spellParticleRenderer.material = attackMaterial;
                        spellParticleSystem.transform.position = enemy.transform.position;
                        spellParticleSystem.transform.position += Vector3.up * 1.0f;
                        spellParticleSystem.Play();
                        enemy.Attack();
                    }
                }
            }
        }
    }

    private void OnCameraCreate(CameraMovement cam)
    {
        this.cameraMovement = cam;
        this.cam = cameraMovement.GetComponent<Camera>();
    }
}
