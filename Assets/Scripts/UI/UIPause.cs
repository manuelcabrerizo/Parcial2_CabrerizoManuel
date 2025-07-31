using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    public static event Action onResumeButtonClick;
    public static event Action onSettingsButtonClick;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        SubscribeButtons();
    }

    private void OnDestroy()
    {
        UnsubscribeButtons();
    }

    private void OnResumeButtonClick()
    {
        onResumeButtonClick?.Invoke();
    }

    private void OnSettingsButtonClick()
    {
        onSettingsButtonClick?.Invoke();
    }

    private void OnMenuButtonClick()
    {
        UnsubscribeButtons();
        GameSceneManager.Instance.ChangeSceneTo("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
        GameManager.onShowLoadingBar?.Invoke(true);
    }

    private void OnExitButtonClick()
    {
#if UNITY_WEBGL
        return;
#endif
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SubscribeButtons()
    {
        resumeButton.onClick.AddListener(OnResumeButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        menuButton.onClick.AddListener(OnMenuButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void UnsubscribeButtons()
    {
        resumeButton.onClick.RemoveListener(OnResumeButtonClick);
        settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
        menuButton.onClick.RemoveListener(OnMenuButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }
}
