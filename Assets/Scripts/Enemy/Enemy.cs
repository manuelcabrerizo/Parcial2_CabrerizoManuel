using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public static event Action<Enemy> onEnemySpawn;
    public static event Action<Enemy> onEnemyKill;

    [SerializeField] protected float life;
    protected Transform target = null;

    public Transform Target => target;


    private void Awake()
    {
        OnAwaken();
    }

    private void Start()
    {
        onEnemySpawn?.Invoke(this);
        OnStart();
    }

    private void OnDestroy()
    {
        OnDestroyed();
    }

    protected virtual void OnAwaken() { }

    protected virtual void OnStart() { }

    protected virtual void OnDestroyed() { }

    public void SetTarget(Transform target)
    { 
        this.target = target;
    }

    public virtual void TakeDamage(int amount)
    {
        life = Mathf.Max(life - amount, 0);
        if (life == 0)
        {
            onEnemyKill?.Invoke(this);
        }
    }
}
