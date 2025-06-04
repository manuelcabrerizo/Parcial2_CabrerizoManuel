using UnityEngine;

public abstract class Spawner<SpawnerType, SpawnObjectBaseType> : MonoBehaviourSingleton<SpawnerType>
    where SpawnerType : MonoBehaviourSingleton<SpawnerType>
    where SpawnObjectBaseType : MonoBehaviour, IPooleable
{
    public T Spawn<T>() where T : SpawnObjectBaseType
    {
        T go = PoolManager.Instance.Get<T>(transform);
        return go;
    }
    public void Release<T>(T go) where T : SpawnObjectBaseType
    {
        PoolManager.Instance.Release(go);
    }
    public void Clear<T>() where T : SpawnObjectBaseType
    {
        PoolManager.Instance.Clear<T>();
    }
}
