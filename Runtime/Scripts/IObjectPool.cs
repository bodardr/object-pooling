namespace Bodardr.ObjectPooling
{
    public interface IObjectPoolGet<T>
    {
        /// <summary>
        ///     Gets a pooled object.
        /// </summary>
        /// <returns>
        ///     A poolable object,
        ///     if empty, returns null.
        /// </returns>
        PoolableObject<T> Get();
    }

    public interface IObjectPool<T>
    {
        void Retrieve(PoolableObject<T> poolableObject);
        bool Contains(PoolableObject<T> poolableObject);
        bool Contains(T poolableObject);
    }
}