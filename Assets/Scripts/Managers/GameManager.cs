using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    private Controlable controlable = null;
    private SceneReferences main = null;
    private string currentLoadedSceneName = null;
    private Transform targetTransform = null;


    protected override void OnAwaken()
    {
        Application.targetFrameRate = 60;
        //QualitySettings.vSyncCount = 1;
        //QualitySettings.SetQualityLevel(0);
        SceneReferences.onLoaded += OnSceneReferencesLoaded;
        Portal.onPortalToSceneEnter += OnPortalToSceneEnter;
        Portal.onPortalToMainEnter += OnPortalToMainEnter;
        Controlable.onControlableCreated += OnControlableCreated;
    }

    protected override void OnDestroyed()
    {
        SceneReferences.onLoaded -= OnSceneReferencesLoaded;
        Portal.onPortalToSceneEnter -= OnPortalToSceneEnter;
        Portal.onPortalToMainEnter -= OnPortalToMainEnter;
        Controlable.onControlableCreated -= OnControlableCreated;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_WEBGL
        return;
#endif
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void OnSceneReferencesLoaded(SceneReferences scene)
    {
        if (main == null)
        {
            main = scene;
        }
    }

    private void OnPortalToSceneEnter(GameObject go, string sceneName, Transform targetTransform)
    {
        if (go == controlable.gameObject)
        {
            GameSceneManager.onLoadingCompleted += OnSceneLoadingComplete;
            GameSceneManager.Instance.ChangeSceneTo(sceneName);
            currentLoadedSceneName = sceneName;
            this.targetTransform = targetTransform;
        }
    }

    private void OnPortalToMainEnter(GameObject go, Transform targetTransform)
    {
        if (go == controlable.gameObject)
        {
            controlable.transform.position = targetTransform.position;
            controlable.transform.rotation = targetTransform.rotation;
            main.SetActiveGo(true);
            SceneManager.UnloadSceneAsync(currentLoadedSceneName);
        }
    }

    private void OnSceneLoadingComplete()
    {
        main.SetActiveGo(false);
        controlable.transform.position = targetTransform.position;
        controlable.transform.rotation = targetTransform.rotation;
        GameSceneManager.onLoadingCompleted -= OnSceneLoadingComplete;

    }

    private void OnControlableCreated(Controlable controlable)
    {
        this.controlable = controlable;
    }

    

    


}
