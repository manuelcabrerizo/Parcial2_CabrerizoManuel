using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    public static event Action<SceneReferences> onLoaded;

    [SerializeField] public Transform targetTransform;
    [SerializeField] private List<GameObject> gameObjects;
    [SerializeField] private List<GameObject> controlables;

    private void Start()
    {
        onLoaded?.Invoke(this);
    }

    public void SetActiveGo(bool value)
    {
        foreach (GameObject go in gameObjects)
        {
            go.SetActive(value);   
        }
    }

    public void SetActiveControlables(bool value, GameObject ignoreObject)
    {
        foreach (GameObject go in controlables)
        {
            if (go != ignoreObject)
            {
                go.SetActive(value);
            }
        }
    }
}
