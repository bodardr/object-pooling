using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bodardr.ObjectPooling
{
    [CreateAssetMenu(fileName = "Prefab Pool", menuName = "Object Pool/Prefab")]
    public class PrefabPool : ScriptableObject, IObjectPool<GameObject>
    {
        private Stack<PoolableObject<GameObject>> pool;

        [SerializeField]
        protected GameObject prefab;

        [SerializeField]
        protected int baseInstantiation = 10;

        public GameObject Prefab => prefab;

        public int BaseInstantiation => baseInstantiation;

        protected void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneChanged;
            InstantiatePool();
        }


        protected void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneChanged;
            DestroyPool();
        }

        public virtual void Retrieve(PoolableObject<GameObject> poolable)
        {
            if (poolable == null || poolable.Content == null)
                return;
            
            poolable.Content.SetActive(false);
            pool.Push(poolable);
        }

        public bool Contains(PoolableObject<GameObject> poolable)
        {
            return pool.Contains(poolable);
        }

        public bool Contains(GameObject poolableObject)
        {
            return pool.Any(x => x.Content == poolableObject);
        }

        private void OnSceneChanged(Scene unloadedScene)
        {
            if (pool == null)
                return;

            foreach (var element in pool.Where(element => element.ReferencedScene == unloadedScene))
                element.Release();
        }

        public PoolableObject<GameObject> Get(Transform parent = null)
        {
            if (pool == null || pool.Count < 1 || pool.Peek().Content == null)
                InstantiatePool();

            PoolableObject<GameObject> gameObject;
            
            if (pool.Count == 0)
            {
                Debug.Log("Pool base exceeded, consider increasing the pool.");
                gameObject = InstantiateSingleObject();
            }
            else
            {
                gameObject =  pool.Pop();
            }

            gameObject.Content.transform.SetParent(parent);
            gameObject.Content.SetActive(true);
            return gameObject;
        }

        public PoolableObject<GameObject> Get(Vector3 position)
        {
            var poolableObject = Get();

            var tr = poolableObject.Content.transform;
            tr.position = position;

            return poolableObject;
        }

        public PoolableObject<GameObject> Get(Vector3 position, Quaternion rotation)
        {
            var poolableObject = Get();
            
            var tr = poolableObject.Content.transform;
            tr.SetPositionAndRotation(position, rotation);
            
            return poolableObject;
        }

        public PoolableComponent<T> Get<T>(Transform parent = null)
        {
            var poolableObject = Get(parent);

            return new PoolableComponent<T>(poolableObject.Content.GetComponent<T>(), poolableObject);
        }

        public PoolableComponent<T> Get<T>(Vector3 position) where T : Component
        {
            var poolableObject = Get();
            poolableObject.Content.transform.position = position;
            
            return new PoolableComponent<T>(poolableObject.Content.GetComponent<T>(), poolableObject);
        }

        public PoolableComponent<T> Get<T>(Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var poolableObject = Get(position, rotation);
            poolableObject.Content.transform.SetParent(parent);
            
            return new PoolableComponent<T>(poolableObject.Content.GetComponent<T>(), poolableObject);
        }

        public T Get<T>(out PoolableObject<GameObject> go) where T : Component
        {
            if (pool == null)
                InstantiatePool();

            if (pool.Count == 0)
            {
                Debug.Log("Pool base exceeded, consider increasing the pool.");
                go = InstantiateSingleObject();
            }
            else
            {
                go = Get();
            }

            return go.Content.GetComponent<T>();
        }

        protected virtual void InstantiatePool()
        {
            if (!Application.isPlaying)
                return;

            pool = new Stack<PoolableObject<GameObject>>();

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
                var element = pool.Pop();

#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(element.Content);
#else
            Destroy(element.Content);
#endif
            }

            pool = null;
        }
    }
}