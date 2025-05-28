using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static event Action<string, Transform> onPortalEnter;
    public static event Action onPortalToMainEnter;

    [SerializeField] private bool isPortalToMain;
    [SerializeField] private string sceneToLoadName;
    [SerializeField] private Transform exitTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (isPortalToMain)
        {
            onPortalToMainEnter.Invoke();
        }
        else
        {
            onPortalEnter?.Invoke(sceneToLoadName, exitTransform);
        }
    }
}
