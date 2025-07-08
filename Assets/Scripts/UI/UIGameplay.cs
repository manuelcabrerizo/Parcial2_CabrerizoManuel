using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private Image lifeBarImage;
    [SerializeField] private Image manaBarImage;

    private void Awake()
    {
        Player.onLifeChange += OnLifeChange;
        Player.onManaChange += OnManaChange;
    }

    private void OnDestroy()
    {
        Player.onLifeChange -= OnLifeChange;
        Player.onManaChange -= OnManaChange;
    }

    private void OnLifeChange(int life, int maxLife)
    {
        lifeBarImage.fillAmount = (float)life / (float)maxLife;
    }

    private void OnManaChange(float mana, float maxMana)
    {
        manaBarImage.fillAmount = mana / maxMana;
    }
}
