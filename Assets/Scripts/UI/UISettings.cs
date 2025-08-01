using System;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    public static event Action<float> onMusicSliderChange;
    public static event Action<float> onSfxSliderChange;
    public static event Action<float> onUISliderChange;
    public static event Action onBackButtonClick;

    // Settings ui
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Button backButton;
    [SerializeField] private VolumeDataSO volumeData;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
        musicSlider.onValueChanged.AddListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChange);
        uiSlider.onValueChanged.AddListener(OnUISliderChange);
    }

    private void Start()
    {
        musicSlider.value = volumeData.Music;
        sfxSlider.value = volumeData.Sfx;
        uiSlider.value = volumeData.UI;
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClick);
        musicSlider.onValueChanged.RemoveListener(OnMusicSliderChange);
        sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChange);
        uiSlider.onValueChanged.RemoveListener(OnUISliderChange);
    }

    private void OnMusicSliderChange(float value)
    {
        onMusicSliderChange?.Invoke(value);
    }
    private void OnSfxSliderChange(float value)
    {
        onSfxSliderChange?.Invoke(value);
    }

    private void OnUISliderChange(float value)
    {
        onUISliderChange?.Invoke(value);
    }
    private void OnBackButtonClick()
    {
        onBackButtonClick?.Invoke();
    }
}
