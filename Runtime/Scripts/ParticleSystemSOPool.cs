using System.Collections;
using System.Collections.Generic;
using Bodardr.UI.Runtime;
using UnityEngine;

namespace Bodardr.ObjectPooling
{
    [CreateAssetMenu(fileName = "Particle System Pool", menuName = "Pool/Particle System Prefab")]
    public class ParticleSystemSOPool : SOPool
    {
        [Tooltip("When Release is called, the object usually disables immediately. " +
                 "If set to true, the pool will wait until the particles have finished emitting before disabling it.")]
        [SerializeField]
        private bool waitForEmissionBeforeDisabling = true;

        [ShowIf(nameof(waitForEmissionBeforeDisabling))]
        [SerializeField]
        private bool useCustomWaitDuration = true;

        [SerializeField]
        private float customWaitDuration = 2f;

        private Dictionary<GameObject, ParticleSystem> particleSystems;

        public override PoolableObject<GameObject> Get()
        {
            var gameObject = base.Get();

            var pe = particleSystems[gameObject.Content];
            pe.Clear();
            pe.Play();

            return gameObject;
        }

        protected override PoolableObject<GameObject> InstantiateSingleObject(int index = 0)
        {
            var gameObject = base.InstantiateSingleObject(index);

            particleSystems.Add(gameObject.Content, gameObject.Content.GetComponent<ParticleSystem>());

            return gameObject;
        }

        protected override void InstantiatePool()
        {
            particleSystems = new Dictionary<GameObject, ParticleSystem>();
            base.InstantiatePool();
        }

        protected override void DestroyPool()
        {
            particleSystems = null;
            base.DestroyPool();
        }

        public override void Retrieve(PoolableObject<GameObject> poolable)
        {
            if (waitForEmissionBeforeDisabling)
            {
                if (!poolable.Content.TryGetComponent(out ParticleSystemDelayBehavior mono))
                    mono = poolable.Content.AddComponent<ParticleSystemDelayBehavior>();

                mono!.StartCoroutine(WaitForEmissionThenReleaseCoroutine(poolable));
            }
            else
            {
                base.Retrieve(poolable);
            }
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