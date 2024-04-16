using UnityEngine;

namespace Bodardr.ObjectPooling
{
    public static class ScriptableObjectPoolExtensions
    {
        public static PoolableComponent<T> Get<T>(this ScriptableObjectPrefabPool prefabPool, Transform parent = null)
        {
            var poolableObject = prefabPool.Get(parent);

            return new PoolableComponent<T>(poolableObject);
        }

        public static PoolableComponent<T> Get<T>(this ScriptableObjectPrefabPool prefabPool, Vector3 position) where T : Component
        {
            var poolableObject = prefabPool.Get(position);
            poolableObject.Content.transform.position = position;

            return new PoolableComponent<T>(poolableObject);
        }

        public static PoolableComponent<T> Get<T>(this ScriptableObjectPrefabPool prefabPool, Vector3 position, Quaternion rotation,
            Transform parent = null) where T : Component
        {
            var poolableObject = prefabPool.Get(position, rotation);

            if (!parent)
                return new PoolableComponent<T>(poolableObject);

            poolableObject.Content.transform.SetParent(parent);
            poolableObject.Content.transform.localPosition = Vector3.zero;

            return new PoolableComponent<T>(poolableObject);
        }
    }
}