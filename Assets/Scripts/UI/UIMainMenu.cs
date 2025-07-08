using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject loadingBar;
    [SerializeField] private Image lodingBarImage;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        playButton.onClick.AddListener(OnPlayButtonClick);
        controlsButton.onClick.AddListener(OnControlsButtonClick);
        creditsButton.onClick.AddListener(OnCreditsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        GameSceneManager.onLoadingBarChange += OnLoadingBarChange;
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClick);
        controlsButton.onClick.RemoveListener(OnControlsButtonClick);
        creditsButton.onClick.RemoveListener(OnCreditsButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
        GameSceneManager.onLoadingBarChange -= OnLoadingBarChange;

    }

    private void OnPlayButtonClick()
    {
        loadingBar.SetActive(true);
        GameSceneManager.Instance.ChangeSceneTo("Main", LoadSceneMode.Single);
    }

    private void OnControlsButtonClick()
    {
        SceneManager.LoadScene("Controls");
    }

    private void OnCreditsButtonClick()
    {
        SceneManager.LoadScene("Credits");
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
}
