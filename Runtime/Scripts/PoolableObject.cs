using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bodardr.ObjectPooling
{
    public class PoolableObject<T> : IPoolableObject
    {
        private readonly IObjectPool<T> pool;

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
}