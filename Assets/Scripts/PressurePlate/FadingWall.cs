using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingWall : MonoBehaviour
{
    [SerializeField] private List<PressurePlate> pressurePlates = null;
    [SerializeField] private Transform cameraTransform = null;
    [SerializeField] private Light effectLight = null;

    private Collider collision = null;
    private MeshRenderer meshRenderer = null;
    private Camera cam = null;
    private CameraMovement cameraMovement = null;

    private bool isOpen = false;

    private void Awake()
    {
        cam = Camera.main;
        cameraMovement = cam.GetComponent<CameraMovement>();
        collision = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        int count = 0;
        foreach (PressurePlate pressurePlate in pressurePlates)
        {
            if (pressurePlate.IsPressed())
            {
                count++;
            }
        }

        if (!isOpen && count == pressurePlates.Count)
        {
            isOpen = true;
            collision.enabled = false;
            StartCoroutine(PlayOpenDoorAnimation());
        }


        if (isOpen && count != pressurePlates.Count)
        {
            isOpen = false;
            collision.enabled = true;
            StartCoroutine(PlayCloseDoorAnimation());
        }
    }

    private IEnumerator PlayOpenDoorAnimation()
    {
        cameraMovement.enabled = false;
        
        cam.transform.position = cameraTransform.position;
        cam.transform.rotation = cameraTransform.rotation;

        meshRenderer.material.SetFloat("_Fading", 1.0f);

        float time = 0.0f;
        while (time <= 2.0f)
        {
            float t = Mathf.Min((time / 1.25f), 1.0f);
            effectLight.intensity = Mathf.Lerp(5.0f, 2.0f, t);
            meshRenderer.material.SetFloat("_Fading", 1.0f - t);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        cameraMovement.enabled = true;
    }

    private IEnumerator PlayCloseDoorAnimation()
    {
        cameraMovement.enabled = false;

        cam.transform.position = cameraTransform.position;
        cam.transform.rotation = cameraTransform.rotation;

        meshRenderer.material.SetFloat("_Fading", 0.0f);

        float time = 0.0f;
        while (time <= 2.0f)
        {
            float t = Mathf.Min((time / 1.25f), 1.0f);
            effectLight.intensity = Mathf.Lerp(2.0f, 5.0f, t);
            meshRenderer.material.SetFloat("_Fading", t);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }

        cameraMovement.enabled = true;
    }
}
