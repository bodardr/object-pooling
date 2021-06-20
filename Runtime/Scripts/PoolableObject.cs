using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolableObject<T>
{
    private IObjectPool<T> pool;

    public PoolableObject(T content, IObjectPool<T> pool)
    {
        Content = content;
        this.pool = pool;
    }

    public T Content { get; }

    public Scene ReferencedScene { get; set; }

    public virtual void Release()
    {
        if (pool.Contains(this))
            Debug.Log("Object already disposed.");

        pool.Retrieve(this);
    }
}