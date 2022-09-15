using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bodardr.ObjectPooling
{
    public class PoolableComponent<T> : IPoolableObject
    {
        private readonly PoolableObject<GameObject> gameObject;

        public PoolableComponent(PoolableObject<GameObject> gameObject)
        {
            Content = gameObject.Content.GetComponent<T>();
            this.gameObject = gameObject;
        }

        public T Content { get; set; }

        public Scene ReferencedScene { get; set; }

        public void Release()
        {
            gameObject.Release();
        }
    }
}