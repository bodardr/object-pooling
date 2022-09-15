using UnityEngine;

namespace Bodardr.ObjectPooling
{
    public static class SOPoolExtensions
    {
        public static PoolableComponent<T> Get<T>(this SOPool pool, Transform parent = null)
        {
            var poolableObject = pool.Get(parent);

            return new PoolableComponent<T>(poolableObject);
        }

        public static PoolableComponent<T> Get<T>(this SOPool pool, Vector3 position) where T : Component
        {
            var poolableObject = pool.Get(position);
            poolableObject.Content.transform.position = position;

            return new PoolableComponent<T>(poolableObject);
        }

        public static PoolableComponent<T> Get<T>(this SOPool pool, Vector3 position, Quaternion rotation,
            Transform parent = null) where T : Component
        {
            var poolableObject = pool.Get(position, rotation);
            poolableObject.Content.transform.SetParent(parent);

            return new PoolableComponent<T>(poolableObject);
        }
    }
}