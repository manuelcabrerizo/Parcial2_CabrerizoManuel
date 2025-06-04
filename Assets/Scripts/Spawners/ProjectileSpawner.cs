using UnityEngine;

public class ProjectileSpawner : Spawner<ProjectileSpawner, Projectile>
{
    [SerializeField] private CrateProjectile cratePrefab;
    [SerializeField] private int initialCrateCount = 10;
    
    protected override void OnAwaken()
    {
        PoolManager.Instance.InitPool(cratePrefab, transform, initialCrateCount);
        Projectile.onProjectileRelease += OnProjectileRelease;
    }

    protected override void OnDestroyed()
    {
        Projectile.onProjectileRelease -= OnProjectileRelease;
    }

    private void OnProjectileRelease(Projectile projectile)
    {
        Projectile test = null;
        if (test = projectile as CrateProjectile)
        {
            PoolManager.Instance.Release((CrateProjectile)projectile);
        }
    }
}