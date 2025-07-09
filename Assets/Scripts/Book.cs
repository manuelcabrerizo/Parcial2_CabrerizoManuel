using UnityEngine;

public class Book : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, Time.unscaledDeltaTime * 50.0f, 0);
    }
}
