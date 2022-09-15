namespace Bodardr.ObjectPooling
{
    public interface IPool<T>
    {
        public bool Contains(T poolable);
        public T Get();
        internal void Retrieve(T poolable);
    }
}