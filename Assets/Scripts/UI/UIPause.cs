using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    public static event Action onResumeButtonClick;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        PauseState.onPauseStateEnter += OnPauseStateEnter;
        PauseState.onPauseStateExit += OnPauseStateExit;
        SubscribeButtons();
    }

    private void OnDestroy()
    {
        PauseState.onPauseStateEnter -= OnPauseStateEnter;
        PauseState.onPauseStateExit -= OnPauseStateExit;
        UnsubscribeButtons();
    }

    private void OnPauseStateEnter()
    { 
        pausePanel.SetActive(true);
    }

    private void OnPauseStateExit()
    {
        pausePanel?.SetActive(false);
    }

    private void OnResumeButtonClick()
    {
        onResumeButtonClick?.Invoke();
    }

    private void OnSettingsButtonClick()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
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
