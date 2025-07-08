using UnityEngine;
using UnityEngine.UI;

public class UIWin : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        WinState.onWinStateEnter += OnWinStateEnter;
        WinState.onWinStateExit += OnWinStateExit;
        menuButton.onClick.AddListener(OnMenuButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void OnDestroy()
    {
        WinState.onWinStateEnter -= OnWinStateEnter;
        WinState.onWinStateExit -= OnWinStateExit;
        menuButton.onClick.RemoveListener(OnMenuButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void OnWinStateEnter()
    {
        winPanel.SetActive(true);
    }

    private void OnWinStateExit() 
    {
        winPanel.SetActive(false);
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
