using System.Collections.Generic;
using System.Linq;

namespace Bodardr.ObjectPooling
{
    public class ObjectPool<T> : IObjectPool<T>
    {
        private Stack<PoolableObject<T>> pool;

        public bool Contains(PoolableObject<T> poolableObject)
        {
            return pool.Contains(poolableObject);
        }

        public bool Contains(T poolableObject)
        {
            return pool.Any(x => x.Content.Equals(poolableObject));
        }

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
}