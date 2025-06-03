using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private PlayerMovement player = null;
    private SceneReferences main = null;
    private SceneReferences current = null;
    private string currentLoadedSceneName;

    protected override void OnAwaken()
    {
        PlayerMovement.onPlayerCreated += OnPlayerCreated;
        SceneReferences.onLoaded += OnSceneReferencesLoaded;
        Portal.onPortalEnter += OnPortalEnter;
        Portal.onPortalToMainEnter += OnPortalToMainEnter;
    }

    protected override void OnDestroyed()
    {
        PlayerMovement.onPlayerCreated -= OnPlayerCreated;
        SceneReferences.onLoaded -= OnSceneReferencesLoaded;
        Portal.onPortalEnter -= OnPortalEnter;
        Portal.onPortalToMainEnter -= OnPortalToMainEnter;
    }

    private void OnPlayerCreated(PlayerMovement player)
    {
        this.player = player;
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
            player.transform.position = current.targetTransform.position;
            player.transform.rotation = current.targetTransform.rotation;
        }
    }

    private void OnPortalEnter(string sceneName, Transform exitTransform)
    {
        player.ClearMovement();
        main.targetTransform = exitTransform;
        GameSceneManager.onLoadingCompleted += OnSceneLoadingComplete;
        currentLoadedSceneName = sceneName;
        GameSceneManager.Instance.ChangeSceneTo(sceneName);
    }

    private void OnPortalToMainEnter()
    {
        player.ClearMovement();
        player.transform.position = main.targetTransform.position;
        player.transform.rotation = main.targetTransform.rotation;
        main.SetActiveGo(true);
        GameSceneManager.onLoadingCompleted -= OnSceneLoadingComplete;
        SceneManager.UnloadSceneAsync(currentLoadedSceneName);
    }

    private void OnSceneLoadingComplete()
    {
        main.SetActiveGo(false);
    }

}
