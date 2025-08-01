using UnityEngine;
using UnityEngine.EventSystems;

public class UIOnEnter : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private SoundClipsSO clips;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.onPlayClip?.Invoke(clips.select, ClipType.UI);
    }
}
