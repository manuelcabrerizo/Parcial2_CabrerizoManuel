using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        GameOverState.onGameOverStateEnter += OnGameOverStateEnter;
        GameOverState.onGameOverStateExit += OnGameOverStateExit;
        menuButton.onClick.AddListener(OnMenuButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDestroy()
    {
        GameOverState.onGameOverStateEnter -= OnGameOverStateEnter;
        GameOverState.onGameOverStateExit -= OnGameOverStateExit;
        menuButton.onClick.RemoveListener(OnMenuButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnGameOverStateEnter()
    {
        gameOverPanel.SetActive(true);
    }

    private void OnGameOverStateExit()
    {
        gameOverPanel.SetActive(false);
    }

    private void OnMenuButtonClick()
    {
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
}
