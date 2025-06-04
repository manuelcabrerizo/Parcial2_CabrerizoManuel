using System;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooleable
{
    public static event Action<Projectile> onProjectileRelease;

    public void OnGet()
    {
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void SendReleaseEvent()
    {
        onProjectileRelease?.Invoke(this);
    }
}
