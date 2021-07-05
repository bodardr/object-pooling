using System.Collections.Generic;

public class ObjectPool<T> : IObjectPool<T>
{
    private Stack<PoolableObject<T>> pool;

    public bool Contains(PoolableObject<T> poolableObject) => pool.Contains(poolableObject);

    public PoolableObject<T> Get()
    {
        if (pool.Count > 0)
            return pool.Pop();

        return null;
    }

    public void Retrieve(PoolableObject<T> disposedObject)
    {
        pool.Push(disposedObject);
    }
}