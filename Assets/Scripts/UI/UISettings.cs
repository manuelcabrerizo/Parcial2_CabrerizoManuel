using System;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    public static event Action<float> onMusicSliderChange;
    public static event Action<float> onSfxSliderChange;

    // Settings ui
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private VolumeDataSO volumeData;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChange);
    }

    private void Start()
    {
        musicSlider.value = volumeData.Music;
        sfxSlider.value = volumeData.Sfx;
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClick);
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChange);
    }

    private void OnMusicSliderChange(float value)
    {
        onMusicSliderChange?.Invoke(value);
    }
    private void OnSfxSliderChange(float value)
    {
        onSfxSliderChange?.Invoke(value);
    }

    private void OnBackButtonClick()
    { 
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
