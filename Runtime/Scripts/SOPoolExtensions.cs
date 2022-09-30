using UnityEngine;

namespace Bodardr.ObjectPooling
{
    public static class SOPoolExtensions
    {
        public static PoolableComponent<T> Get<T>(this ScriptableObjectPool pool, Transform parent = null)
        {
            var poolableObject = pool.Get(parent);

            return new PoolableComponent<T>(poolableObject);
        }

        public static PoolableComponent<T> Get<T>(this ScriptableObjectPool pool, Vector3 position) where T : Component
        {
            var poolableObject = pool.Get(position);
            poolableObject.Content.transform.position = position;

            return new PoolableComponent<T>(poolableObject);
        }

        public static PoolableComponent<T> Get<T>(this ScriptableObjectPool pool, Vector3 position, Quaternion rotation,
            Transform parent = null) where T : Component
        {
            var poolableObject = pool.Get(position, rotation);

            if (!parent)
                return new PoolableComponent<T>(poolableObject);

            poolableObject.Content.transform.SetParent(parent);
            poolableObject.Content.transform.localPosition = Vector3.zero;

            return new PoolableComponent<T>(poolableObject);
        }
    }
}