using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private SceneReferences main = null;
    private SceneReferences current = null;
    private string currentLoadedSceneName;

    /*
    protected override void OnAwaken()
    {
        SceneReferences.onLoaded += OnSceneReferencesLoaded;
        Portal.onPortalEnter += OnPortalEnter;
        Portal.onPortalToMainEnter += OnPortalToMainEnter;
    }

    protected override void OnDestroyed()
    {
        SceneReferences.onLoaded -= OnSceneReferencesLoaded;
        Portal.onPortalEnter -= OnPortalEnter;
        Portal.onPortalToMainEnter -= OnPortalToMainEnter;
    }
    
    private void OnEntityCreated(ControlableMovement entity)
    {
        this.entity = entity;
    }

    private void OnSceneReferencesLoaded(SceneReferences scene)
    {
        if (main == null)
        {
            main = scene;
        }
        else
        {
            current = scene;
            entity.transform.position = current.targetTransform.position;
            entity.transform.rotation = current.targetTransform.rotation;
        }
    }

    private void OnPortalEnter(GameObject go, string sceneName, Transform exitTransform)
    {
        if (go == entity.gameObject)
        {
            entity.ClearMovement();
            main.targetTransform = exitTransform;
            GameSceneManager.onLoadingCompleted += OnSceneLoadingComplete;
            currentLoadedSceneName = sceneName;
            GameSceneManager.Instance.ChangeSceneTo(sceneName);
        }
    }

    private void OnPortalToMainEnter(GameObject go)
    {
        if (go == entity.gameObject)
        {
            entity.ClearMovement();
            entity.transform.position = main.targetTransform.position;
            entity.transform.rotation = main.targetTransform.rotation;
            main.SetActiveGo(true);
            main.SetActiveControlables(true, entity.gameObject);
            GameSceneManager.onLoadingCompleted -= OnSceneLoadingComplete;
            SceneManager.UnloadSceneAsync(currentLoadedSceneName);
        }
    }

    private void OnSceneLoadingComplete()
    {
        main.SetActiveGo(false);
        main.SetActiveControlables(false, entity.gameObject);
    }
    */

}
