using System.Collections.Generic;
using System.Linq;

namespace Bodardr.ObjectPooling
{
    public class Pool<T> : IPool<T>
    {
        private Stack<T> pool;

        public bool Contains(T poolable) => pool.Any(x => x.Equals(poolable));

        public T Get()
        {
            if (pool.Count > 0)
                return pool.Pop();

            return default;
        }

        void IPool<T>.Retrieve(T poolable)
        {
            pool.Push(poolable);
        }
    }
}