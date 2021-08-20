using UnityEngine.SceneManagement;

namespace Bodardr.ObjectPooling
{
    public interface IPoolableObject
    {
        Scene ReferencedScene { get; set; }

        void Release();
    }
}