using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bodardr.ObjectPooling
{
    public class PoolableComponent<T> : IPoolableObject where T : Component
    {
        private readonly PoolableObject<GameObject> gameObject;

        public Scene ReferencedScene { get; set; }
        public T Content { get; set; }


        public PoolableComponent(T content, PoolableObject<GameObject> gameObject)
        {
            Content = content;
            this.gameObject = gameObject;
        }

        public void Release()
        {
            gameObject.Release();
        }
    }
}