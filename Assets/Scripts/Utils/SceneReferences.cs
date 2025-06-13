using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    public static event Action<SceneReferences> onLoaded;

    [SerializeField] private List<GameObject> gameObjects;

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
}
