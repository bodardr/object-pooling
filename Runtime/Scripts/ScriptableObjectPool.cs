using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Prefab Pool", menuName = "Object Pool/Prefab")]
public class ScriptableObjectPool<T> : ScriptableObject, IObjectPool<T> where T : Object
{
    [SerializeField]
    protected GameObject prefab;

    [SerializeField]
    protected int baseInstantiation = 10;

    private Stack<PoolableObject<T>> pool;

    protected void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneChanged;
        InstantiatePool();
    }


    protected void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneChanged;
        DestroyPool();
    }

    public virtual void Retrieve(PoolableObject<T> poolable) => pool.Push(poolable);

    public bool Contains(PoolableObject<T> poolable) => pool.Contains(poolable);

    private void OnSceneChanged(Scene unloadedScene)
    {
        if (pool == null)
            return;

        foreach (var element in pool.Where(element => element.ReferencedScene == unloadedScene))
            element.Release();
    }

    public PoolableObject<T> Get()
    {
        if (pool == null)
            InstantiatePool();

        if (pool.Count == 0)
        {
            Debug.Log("Pool base exceeded, consider increasing the pool.");
            return InstantiateSingleObject();
        }

        return pool.Pop();
    }

    protected virtual void InstantiatePool()
    {
        if (!Application.isPlaying)
            return;

        pool = new Stack<PoolableObject<T>>();

        for (int i = 0; i < baseInstantiation; i++)
        {
            pool.Push(InstantiateSingleObject(i));
        }
    }

    protected virtual PoolableObject<T> InstantiateSingleObject(int index = 0)
    {
        var go = Instantiate(prefab);
        go.name = $"{prefab.name}{(index > 0 ? $" {index}" : "")} (Pooled)";
        go.SetActive(false);
        DontDestroyOnLoad(go);

        return new PoolableObject<T>(go.GetComponent<T>(), this);
    }

    protected virtual void DestroyPool()
    {
        if (pool == null)
            return;

        while (pool.Count > 0)
        {
            var element = pool.Pop();

#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(element.Content);
#else
            Destroy(element.Content);
#endif
        }

        pool = null;
    }
}