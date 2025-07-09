using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public static event Action<GameObject, string, Transform> onPortalToSceneEnter;
    public static event Action<GameObject, Transform> onPortalToMainEnter;

    [SerializeField] private bool isPortalToMain;
    [SerializeField] private string sceneToLoadName;
    [SerializeField] private Transform targetTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Player>(out _))
        {
            return;
        }

        if (isPortalToMain)
        {
            onPortalToMainEnter?.Invoke(other.gameObject, targetTransform);
        }
        else
        {
            onPortalToSceneEnter?.Invoke(other.gameObject, sceneToLoadName, targetTransform);
        }
    }
}
