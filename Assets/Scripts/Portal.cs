using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static event Action<GameObject, string, Transform> onPortalEnter;
    public static event Action<GameObject> onPortalToMainEnter;

    [SerializeField] private bool isPortalToMain;
    [SerializeField] private string sceneToLoadName;
    [SerializeField] private Transform exitTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (isPortalToMain)
        {
            onPortalToMainEnter.Invoke(other.gameObject);
        }
        else
        {
            onPortalEnter?.Invoke(other.gameObject, sceneToLoadName, exitTransform);
        }
    }
}
