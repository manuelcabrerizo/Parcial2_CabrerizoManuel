using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public static event Action onSettingsButtonClick;
    public static event Action onCreditsButtonClick;

    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject loadingBar;
    [SerializeField] private Image lodingBarImage;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        SubscribeButtons();
        GameSceneManager.onLoadingBarChange += OnLoadingBarChange;
    }

    private void OnDestroy()
    {
        UnsubscribeButtons();
        GameSceneManager.onLoadingBarChange -= OnLoadingBarChange;
    }

    private void OnPlayButtonClick()
    {
        UnsubscribeButtons();
        loadingBar.SetActive(true);
        GameSceneManager.Instance.ChangeSceneTo("Main", LoadSceneMode.Single);
    }

    private void OnSettingsButtonClick()
    {
        onSettingsButtonClick?.Invoke();
    }

    private void OnCreditsButtonClick()
    {
        onCreditsButtonClick?.Invoke();
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
        playButton.onClick.AddListener(OnPlayButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        creditsButton.onClick.AddListener(OnCreditsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void UnsubscribeButtons()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClick);
        settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
        creditsButton.onClick.RemoveListener(OnCreditsButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnLoadingBarChange(float value)
    {
        lodingBarImage.fillAmount = value;
    }

}
