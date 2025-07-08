using UnityEngine;
using UnityEngine.SceneManagement;
public class Utils
{
    static public bool CheckCollisionLayer(GameObject gameObject, LayerMask layer)
    {
        bool result = ((1 << gameObject.layer) & layer.value) > 0;
        return result;
    }

    public static float AdjustAngle(float angle)
    {
        while (angle > Mathf.PI) angle -= Mathf.PI * 2.0f;
        while (angle < -Mathf.PI) angle += Mathf.PI * 2.0f;
        return angle;
    }

    public static float GetOrientationFromVector(float currentOrientation, Vector3 velocity)
    {
        velocity.y = 0.0f;
        if (velocity.magnitude > 0.0f)
        {
            velocity.Normalize();
            float angle = Mathf.Atan2(-velocity.x, velocity.z);
            if (angle < 0.0f)
            {
                angle += Mathf.PI * 2.0f;
            }
            return angle;
        }
        else
        {
            return currentOrientation;
        }
    }
    public static float LinearToDecibel(float linear)
    {
        float dB;
        if (linear != 0)
            dB = Mathf.Log10(linear) * 20.0f;
        else
            dB = -144.0f;
        return dB;

    }
}
