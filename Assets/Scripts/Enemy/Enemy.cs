using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static event Action<Enemy> onEnemySpawn;

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

    public void Attack()
    { 
    
    }

    public void SetTarget(Transform target)
    { 
        this.target = target;
    }
}
