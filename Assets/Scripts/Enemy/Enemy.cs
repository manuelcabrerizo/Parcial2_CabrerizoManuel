using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float life;

    private void Awake()
    {
        OnAwaken();
    }

    private void OnDestroy()
    {
        OnDestroyed();
    }

    protected virtual void OnAwaken() { }

    protected virtual void OnDestroyed() { }

    public void Attack()
    { 
    
    }
}
