using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private Image lifeBarImage;
    [SerializeField] private Image manaBarImage;

    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        Player.onLifeChange += OnLifeChange;
        Player.onManaChange += OnManaChange;

        PauseState.onPauseStateEnter += OnPauseStateEnter;
        PauseState.onPauseStateExit += OnPauseStateExit;

        UIPause.onSettingsButtonClick += OnSettingsButtonClick;
        UISettings.onBackButtonClick += OnSettingsBackButtonClick;

        WinState.onWinStateEnter += OnWinStateEnter;
        WinState.onWinStateExit += OnWinStateExit;

        GameOverState.onGameOverStateEnter += OnGameOverStateEnter;
        GameOverState.onGameOverStateExit += OnGameOverStateExit;
    }

    private void OnDestroy()
    {
        Player.onLifeChange -= OnLifeChange;
        Player.onManaChange -= OnManaChange;

        PauseState.onPauseStateEnter -= OnPauseStateEnter;
        PauseState.onPauseStateExit -= OnPauseStateExit;

        UIPause.onSettingsButtonClick -= OnSettingsButtonClick;
        UISettings.onBackButtonClick -= OnSettingsBackButtonClick;

        WinState.onWinStateEnter -= OnWinStateEnter;
        WinState.onWinStateExit -= OnWinStateExit;

        GameOverState.onGameOverStateEnter -= OnGameOverStateEnter;
        GameOverState.onGameOverStateExit -= OnGameOverStateExit; 
    }

    private void OnLifeChange(int life, int maxLife)
    {
        lifeBarImage.fillAmount = (float)life / (float)maxLife;
    }

    private void OnManaChange(float mana, float maxMana)
    {
        manaBarImage.fillAmount = mana / maxMana;
    }

    private void OnPauseStateEnter()
    {
        pausePanel.SetActive(true);
    }

    private void OnPauseStateExit()
    {
        pausePanel.SetActive(false);
    }

    private void OnSettingsButtonClick()
    {
        settingsPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    private void OnSettingsBackButtonClick()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    private void OnWinStateEnter()
    {
        winPanel.SetActive(true);
    }

    private void OnWinStateExit()
    {
        winPanel.SetActive(false);
    }

    private void OnGameOverStateEnter()
    {
        gameOverPanel.SetActive(true);
    }

    private void OnGameOverStateExit()
    {
        gameOverPanel.SetActive(false);
    }
}
