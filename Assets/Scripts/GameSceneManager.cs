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

    public void ChangeSceneTo(string sceneName)
    {
        if (loadingScene != null)
        {
            StopCoroutine(loadingScene);
        }
        loadingScene = LoadingScene(sceneName);
        StartCoroutine(loadingScene);
    }

    private IEnumerator LoadingScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        /*
        float onTime = 0.0f;
        while(onTime < maxTime * 0.5f)
        {
            onTime += Time.deltaTime;
            onLoadingBarChange?.Invoke(onTime/maxTime);
            yield return null;
        }
        */

        while(operation.progress < 0.89f)
        {
            yield return null;
        }
        /*
        while(onTime < maxTime)
        {
            onTime += Time.deltaTime;
            onLoadingBarChange?.Invoke(onTime/maxTime);
            yield return null;
        }
        */

        operation.allowSceneActivation = true;
        loadingScene = null;

        onLoadingCompleted?.Invoke();
    }
}
