using UnityEngine;

public class PressurePlate : Signable
{
    [SerializeField] private SoundClipsSO clips;
    [SerializeField] private Transform buttonTransform;
    private Vector3 restPosition;
    private Vector3 pressPosition;
    private bool isPressed = false;

    private void Awake()
    {
        restPosition = buttonTransform.position;
        pressPosition = buttonTransform.position - Vector3.up * 0.25f;
        isPressed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        buttonTransform.position = pressPosition;
        isPressed = true;
        AudioManager.onPlayClip3D(clips.onPressurePlate, transform.position, 10, 40);
    }

    private void OnTriggerStay(Collider other)
    {
        buttonTransform.position = pressPosition;
        isPressed = true;
    }

    private void OnTriggerExit(Collider other)
    {
        buttonTransform.position = restPosition;
        isPressed = false;
    }

    public override bool IsSignal()
    {
        return isPressed;
    }
}
