using System;
using UnityEngine;
using UnityEngine.UI;

public class UICredits : MonoBehaviour
{
    public static event Action onBackButtonClick;

    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBackButtonClick);
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClick);
    }

    private void OnBackButtonClick()
    {
        onBackButtonClick?.Invoke();
    }
}
