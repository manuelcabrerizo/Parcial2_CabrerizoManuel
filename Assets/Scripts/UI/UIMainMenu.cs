using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject loadingBar;
    [SerializeField] private Image lodingBarImage;

    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button creditsBackButton;

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

    private void OnCreditsButtonClick()
    {
        creditsPanel.SetActive(true);
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

    private void OnLoadingBarChange(float value)
    {
        lodingBarImage.fillAmount = value;
    }


    private void OnCreditsBackButtonClick()
    {
        creditsPanel.SetActive(false);
    }


    private void SubscribeButtons()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        creditsButton.onClick.AddListener(OnCreditsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        creditsBackButton.onClick.AddListener(OnCreditsBackButtonClick);
    }

    private void UnsubscribeButtons()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClick);
        creditsButton.onClick.RemoveListener(OnCreditsButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
        creditsBackButton.onClick.RemoveListener(OnCreditsBackButtonClick);
    }

}
