using System;
using UnityEngine;

namespace Bodardr.ObjectPooling
{
    public class PoolableParticleSystemCallback : MonoBehaviour
    {
        public PoolableObject<GameObject> Poolable { get; set; }

        private void OnParticleSystemStopped()
        {
            Poolable.Release();
        }
    }
}