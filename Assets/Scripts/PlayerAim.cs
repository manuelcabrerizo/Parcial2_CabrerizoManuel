using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private ParticleSystem aimParticleSystem;
    private float mousePosX = 0.0f;
    private float mousePosY = 0.0f;
    private Animator animator;

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
        }
        
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
                    if (go.TryGetComponent<Controlable>(out controlable))
                    {
                        Destroy(GetComponent<EntityMovement>());
                        controlable.Control(player);
                        
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
