public interface IObjectPool<T>
{
    /// <summary>
    /// Gets a pooled object.
    /// </summary>
    /// <returns>A poolable object,
    /// if empty, returns null.</returns>
    PoolableObject<T> Get();

    void Retrieve(PoolableObject<T> poolableObject);
    bool Contains(PoolableObject<T> poolableObject);
}