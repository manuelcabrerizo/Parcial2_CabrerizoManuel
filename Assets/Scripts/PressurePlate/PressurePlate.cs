using UnityEngine;

public class PressurePlate : MonoBehaviour
{
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

    public bool IsPressed()
    {
        return isPressed;
    }
}
