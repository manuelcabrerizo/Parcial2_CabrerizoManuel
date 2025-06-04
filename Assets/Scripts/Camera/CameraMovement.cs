using System;
using TMPro;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public static event Action<CameraMovement> onCameraCreate;

    [SerializeField][Range(-0.6f, 0.8f)] private float heightRatio = 0.25f;
    [SerializeField][Range(0.1f, 20.0f)] private float distance = 10.0f;
    [SerializeField][Range(-10.0f, 10.0f)] private float xOffset = 0.0f;
    [SerializeField][Range(-10.0f, 10.0f)] private float yOffset = 0.0f;
    [SerializeField] private LayerMask wallLayer;

    private float yaw = 0;
    private float currentHeightRatio;
    private bool isAiming = false;

    private GameObject goTraget;


    private void Awake()
    {
        EntityMovement.onEntityCreated += OnEntityCreated;
    }

    private void OnDestroy()
    {
        EntityMovement.onEntityCreated -= OnEntityCreated;
    }

    private void Start()
    {
        currentHeightRatio = heightRatio;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        if (goTraget == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        yaw += mouseX * 1.0f;


        if (Input.GetMouseButton(0))
        {
            currentHeightRatio = Mathf.Clamp(currentHeightRatio + mouseY * 0.025f, -0.6f, 0.8f);
            isAiming = true;
        }
        else
        {
            currentHeightRatio = Mathf.Lerp(currentHeightRatio, heightRatio, Time.deltaTime * 4.0f);
            isAiming = false;
        }

        Vector3 direction = new Vector3(0.0f, 0.0f, -1.0f);
        direction = Quaternion.AngleAxis(yaw, Vector3.up) * direction;
        direction.Normalize();

        Vector3 target = goTraget.transform.position + (direction + Vector3.up * currentHeightRatio).normalized * distance;

        Vector3 viewPosition = goTraget.transform.position;
        target += Vector3.up * yOffset;
        target += transform.right * xOffset;
        viewPosition += Vector3.up * yOffset;
        viewPosition += transform.right * xOffset;

        Vector3 toTargte = target - viewPosition;
        float magnitude = toTargte.magnitude;
        toTargte.Normalize();

        RaycastHit hitInfo;
        if (Physics.SphereCast(viewPosition, 0.2f, toTargte, out hitInfo, magnitude, wallLayer))
        {
            target = viewPosition + toTargte * hitInfo.distance;
        }

        transform.position = target;
        transform.LookAt(viewPosition);
    }

    public float GetYaw()
    {
        return yaw;
    }

    public bool IsAiming()
    {
        return isAiming;
    }

    public void OnEntityCreated(EntityMovement entity)
    {
        goTraget = entity.gameObject;
        onCameraCreate?.Invoke(this);
    }
}
