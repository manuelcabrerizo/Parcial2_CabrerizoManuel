using UnityEngine;
using UnityEngine.UI;

public class LoadingBarManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingBar;
    [SerializeField] private Image lodingBarImage;

    private void Awake()
    {
        GameManager.onShowLoadingBar += OnShowLoadingBar;
        GameSceneManager.onLoadingBarChange += OnLoadingBarChange;
    }

    private void OnDestroy()
    {
        GameManager.onShowLoadingBar -= OnShowLoadingBar;
        GameSceneManager.onLoadingBarChange -= OnLoadingBarChange;
    }

    private void OnShowLoadingBar(bool show)
    { 
        loadingBar.SetActive(show);
    }

    private void OnLoadingBarChange(float value)
    {
        lodingBarImage.fillAmount = value;
    }
}
