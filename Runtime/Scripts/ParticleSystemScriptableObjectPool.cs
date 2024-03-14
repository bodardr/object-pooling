using System;
using System.Collections;
using System.Collections.Generic;
using Bodardr.Utility.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Bodardr.ObjectPooling
{
    [CreateAssetMenu(fileName = "Particle System Pool", menuName = "Pool/Particle System Prefab")]
    public class ParticleSystemScriptableObjectPool : ScriptableObjectPool
    {
        public enum RetrieveStrategy
        {
            WaitForDuration,
            UseOnStopCallback
        }

        [SerializeField]
        private RetrieveStrategy retrieveStrategy;

        [Tooltip("When Release is called, the object usually waits for the max emission time." +
                 "If set to true, the pool will wait until the particles have finished emitting before disabling it.")]
        [SerializeField]
        private bool disableInstantly = true;

        [ShowIf(nameof(disableInstantly))]
        [SerializeField]
        private bool useCustomWaitDuration = true;

        [ShowIf(nameof(useCustomWaitDuration))]
        [SerializeField]
        private float customWaitDuration = 2f;

        private Dictionary<GameObject, Tuple<ParticleSystem, PoolableParticleSystemCallback>> particleSystems;

        public override PoolableObject<GameObject> Get()
        {
            var gameObject = base.Get();

            var pe = particleSystems[gameObject.Content].Item1;

            pe.Clear();
            pe.Play();

            return gameObject;
        }

        protected override PoolableObject<GameObject> InstantiateSingleObject(int index = 0)
        {
            var gameObject = base.InstantiateSingleObject(index);

            var pe = gameObject.Content.GetComponent<ParticleSystem>();

            if (!pe.TryGetComponent(out PoolableParticleSystemCallback callback))
                callback = pe.gameObject.AddComponent<PoolableParticleSystemCallback>();

            callback.Poolable = gameObject;

            particleSystems.Add(gameObject.Content, new(pe, callback));

            return gameObject;
        }

        protected override void InstantiatePool()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif

            particleSystems = new Dictionary<GameObject, Tuple<ParticleSystem, PoolableParticleSystemCallback>>();
            base.InstantiatePool();
        }

        protected override void DestroyPool()
        {
            particleSystems = null;
            base.DestroyPool();
        }

        public override void Retrieve(PoolableObject<GameObject> poolable)
        {
            if (!disableInstantly && poolable.Content != null && poolable.Content.activeInHierarchy)
                particleSystems[poolable.Content].Item2.StartCoroutine(
                    WaitForEmissionThenReleaseCoroutine(poolable));
            else
                base.Retrieve(poolable);
        }

        private IEnumerator WaitForEmissionThenReleaseCoroutine(PoolableObject<GameObject> poolable)
        {
            var pe = poolable.Content.GetComponent<ParticleSystem>();
            pe.Stop();

            //Waits for the max lifetime of the main particles.
            var duration = useCustomWaitDuration ? customWaitDuration : pe.main.startLifetime.constantMax;

            yield return new WaitForSeconds(duration);
            base.Retrieve(poolable);
        }
    }
}