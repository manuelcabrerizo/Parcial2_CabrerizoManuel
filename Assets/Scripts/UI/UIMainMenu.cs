using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject settingsPanel;


    private void Awake()
    {
        UIMenu.onSettingsButtonClick += OnSettingsButtonClick;
        UISettings.onBackButtonClick += OnSettingsBackButtonClick;
        UIMenu.onCreditsButtonClick += OnCreditsButtonClick;
        UICredits.onBackButtonClick += OnCreditsBackButtonClick;
    }

    private void OnDestroy()
    {
        UIMenu.onSettingsButtonClick -= OnSettingsButtonClick;
        UISettings.onBackButtonClick -= OnSettingsBackButtonClick;
        UIMenu.onCreditsButtonClick -= OnCreditsButtonClick;
        UICredits.onBackButtonClick -= OnCreditsBackButtonClick;
    }

    private void OnSettingsButtonClick()
    { 
        settingsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    private void OnSettingsBackButtonClick()
    {
        menuPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OnCreditsButtonClick()
    {
        creditsPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    private void OnCreditsBackButtonClick()
    {
        menuPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }
}
