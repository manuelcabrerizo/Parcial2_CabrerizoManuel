using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviourSingleton<PoolManager>
{
    private Dictionary<Type, IPooleable> prefabs = new Dictionary<Type, IPooleable>();
    private Dictionary<Type, Stack<IPooleable>> freePools = new Dictionary<Type, Stack<IPooleable>>();
    private Dictionary<Type, List<IPooleable>> inUsePools = new Dictionary<Type, List<IPooleable>>();

    public void InitPool<T>(T prefab, Transform parent = null, int count = 0) where T : MonoBehaviour, IPooleable
    {
        Debug.Assert(!prefabs.ContainsKey(typeof(T)), "Pool Already Initialized!");

        prefabs.Add(typeof(T), prefab);
        freePools.Add(typeof(T), new Stack<IPooleable>());
        inUsePools.Add(typeof(T), new List<IPooleable>());
        for (int i = 0; i < count; ++i)
        {
            T go = Instantiate(prefab, parent);
            go.OnRelease();
            freePools[typeof(T)].Push(go);
        }
    }

    public T Get<T>(Transform parent = null) where T : MonoBehaviour, IPooleable
    {
        Debug.Assert(prefabs.ContainsKey(typeof(T)), "Pool Not Initialized!");

        T go = null;
        Stack<IPooleable> freePool = freePools[typeof(T)];
        List<IPooleable> inUsePool = inUsePools[typeof(T)];
        if (freePool.Count == 0)
        {
            MonoBehaviour prefab = (MonoBehaviour)prefabs[typeof(T)];
            go = (T)Instantiate(prefab, parent);
        }
        else
        {
            go = (T)freePool.Pop();
        }
        go.OnGet();
        inUsePool.Add(go);
        return go;
    }

    public void Release<T>(T go) where T : MonoBehaviour, IPooleable
    {
        Debug.Assert(prefabs.ContainsKey(typeof(T)), "This GameObject is not from any of this pools");

        Stack<IPooleable> freePool = freePools[typeof(T)];
        List<IPooleable> inUsePool = inUsePools[typeof(T)];
        go.OnRelease();
        inUsePool.Remove(go);
        freePool.Push(go);
    }

    public void Clear<T>() where T : MonoBehaviour, IPooleable
    {
        Debug.Assert(prefabs.ContainsKey(typeof(T)), "Pool Not Initialized!");

        Stack<IPooleable> freePool = freePools[typeof(T)];
        List<IPooleable> inUsePool = inUsePools[typeof(T)];
        foreach (IPooleable go in inUsePool)
        {
            go.OnRelease();
            freePool.Push(go);
        }
        inUsePool.Clear();
    }
}