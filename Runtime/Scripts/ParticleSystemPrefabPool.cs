using System.Collections.Generic;
using UnityEngine;

namespace Bodardr.ObjectPooling
{
    [CreateAssetMenu(fileName = "Particle System Pool", menuName = "Pool/Particle System Prefab")]
    public class ParticleSystemSOPool : SOPool
    {
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
            base.InstantiatePool();
            particleSystems = new Dictionary<GameObject, ParticleSystem>();
        }

        protected override void DestroyPool()
        {
            base.DestroyPool();
            particleSystems = null;
        }
    }
}