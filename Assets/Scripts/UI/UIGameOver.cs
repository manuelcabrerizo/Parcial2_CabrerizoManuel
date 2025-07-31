using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
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
        menuButton.onClick.AddListener(OnMenuButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void UnsubscribeButtons()
    {
        menuButton.onClick.RemoveListener(OnMenuButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }
}
