using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviourSingleton<GameSceneManager>
{
    public static event Action onLoadingCompleted;
    public static event Action<float> onLoadingBarChange;

    [SerializeField] private float maxTime;
    private IEnumerator loadingScene = null;
    private IEnumerator unloadScene = null;

    public void ChangeSceneTo(string sceneName, LoadSceneMode loadSceneMode)
    {
        if (loadingScene != null)
        {
            StopCoroutine(loadingScene);
        }
        loadingScene = LoadingScene(sceneName, loadSceneMode);
        StartCoroutine(loadingScene);
    }

    public void UnloadScene(string sceneName)
    {
        if (unloadScene != null)
        {
            StopCoroutine(unloadScene);
        }
        unloadScene = UnloadingScene(sceneName);
        StartCoroutine(unloadScene);
    }

    private IEnumerator LoadingScene(string sceneName, LoadSceneMode loadSceneMode)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        operation.allowSceneActivation = false;

        Time.timeScale = 0.0f;

        float onTime = 0.0f;
        while(onTime < maxTime * 0.5f)
        {
            onTime += Time.unscaledDeltaTime;
            onLoadingBarChange?.Invoke(onTime/maxTime);
            yield return null;
        }
        

        while(operation.progress < 0.89f)
        {
            yield return null;
        }
        
        while(onTime < maxTime)
        {
            onTime += Time.unscaledDeltaTime;
            onLoadingBarChange?.Invoke(onTime/maxTime);
            yield return null;
        }
        

        operation.allowSceneActivation = true;
        loadingScene = null;

        Time.timeScale = 1.0f;

        onLoadingCompleted?.Invoke();
    }

    private IEnumerator UnloadingScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);

        operation.allowSceneActivation = false;

        Time.timeScale = 0.0f;

        float onTime = 0.0f;
        while (onTime < maxTime * 0.5f)
        {
            onTime += Time.unscaledDeltaTime;
            onLoadingBarChange?.Invoke(onTime / maxTime);
            yield return null;
        }

        while (operation.progress < 0.89f)
        {
            yield return null;
        }

        while (onTime < maxTime)
        {
            onTime += Time.unscaledDeltaTime;
            onLoadingBarChange?.Invoke(onTime / maxTime);
            yield return null;
        }


        operation.allowSceneActivation = true;
        loadingScene = null;

        Time.timeScale = 1.0f;

        onLoadingCompleted?.Invoke();
    }


}
