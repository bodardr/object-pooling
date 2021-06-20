public interface IObjectPool<T>
{
    void Retrieve(PoolableObject<T> poolableObject);
    bool Contains(PoolableObject<T> poolableObject);
}