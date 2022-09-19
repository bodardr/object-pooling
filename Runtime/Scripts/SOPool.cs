using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bodardr.ObjectPooling
{
    [CreateAssetMenu(fileName = "New Scriptable Object Pool", menuName = "Pool/Prefab")]
    public class SOPool : ScriptableObject, IPool<PoolableObject<GameObject>>
    {
        [SerializeField]
        protected GameObject prefab;

        [SerializeField]
        protected int baseInstantiation = 10;

        protected Stack<PoolableObject<GameObject>> pool = new();

        protected virtual void OnEnable()
        {
            InstantiatePool();

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        protected void OnDisable()
        {
            DestroyPool();

            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public bool Contains(PoolableObject<GameObject> poolable) => pool.Any(x => x == poolable);

        /// <summary>
        /// <b>DO NOT CALL DIRECTLY. You might be looking for PoolableObject's <see cref="PoolableObject{T}.Release"/> instead</b>
        /// Called by <see cref="PoolableObject{T}.Release"/> so it may be reinserted into the pool.
        /// </summary>
        /// <param name="poolable"></param>
        public virtual void Retrieve(PoolableObject<GameObject> poolable)
        {
            if (poolable == null || !poolable.Content)
            {
                Debug.LogWarning("The poolable is null.");
                return;
            }

            pool.Push(poolable);
        }

        public virtual PoolableObject<GameObject> Get()
        {
            if (pool == null || pool.Count < 1 || !pool.Peek().Content)
                InstantiatePool();

            PoolableObject<GameObject> gameObject;

            if (pool!.Count == 0)
            {
                Debug.Log("Pool base exceeded, consider increasing the pool.");
                gameObject = InstantiateSingleObject();
            }
            else
            {
                gameObject = pool.Pop();
                gameObject.ReferencedScene = SceneManager.GetActiveScene();
            }

            gameObject.Content.SetActive(true);
            return gameObject;
        }

        private void OnSceneUnloaded(Scene unloadedScene)
        {
            if (pool == null)
                return;

            foreach (var element in pool.Where(element => element.ReferencedScene == unloadedScene))
                element.Release();
        }

        public PoolableObject<GameObject> Get(Transform parent, bool resetLocalPosition = true)
        {
            var gameObject = Get();

            if (parent)
            {
                gameObject.Content.transform.SetParent(parent);

                if (resetLocalPosition)
                    gameObject.Content.transform.localPosition = Vector3.zero;
            }

            gameObject.Content.SetActive(true);
            return gameObject;
        }

        public PoolableObject<GameObject> Get(Vector3 position)
        {
            var poolableObject = Get();
            poolableObject.Content.transform.position = position;
            return poolableObject;
        }

        public PoolableObject<GameObject> Get(Vector3 position, Quaternion rotation)
        {
            var poolableObject = Get();
            poolableObject.Content.transform.SetPositionAndRotation(position, rotation);
            return poolableObject;
        }

        protected virtual void InstantiatePool()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            pool?.Clear();
            pool ??= new Stack<PoolableObject<GameObject>>();

            for (var i = 0; i < baseInstantiation; i++)
                pool.Push(InstantiateSingleObject(i));
        }

        protected virtual PoolableObject<GameObject> InstantiateSingleObject(int index = 0)
        {
            var go = Instantiate(prefab);
            go.name = $"{prefab.name} {(index > 0 ? $"{index}" : "")} (Pooled)";
            go.SetActive(false);
            DontDestroyOnLoad(go);

            return new PoolableObject<GameObject>(go, this);
        }

        protected virtual void DestroyPool()
        {
            if (pool == null)
                return;

            while (pool.Count > 0)
            {
                var poolableGameObject = pool.Pop();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(poolableGameObject.Content);
#else
            Destroy(element.Content);
#endif
            }

            pool = null;
        }
    }
}