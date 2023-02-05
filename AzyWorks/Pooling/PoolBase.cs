namespace AzyWorks.Pooling
{
    public abstract class PoolBase
    {
        internal abstract void InitPool();
        internal abstract void DestroyPool();
        internal abstract void ResetPool();
        internal abstract void PushObject(object value);

        internal abstract object GetObject();

        internal abstract bool TryGetObject(out object obj);
    }
}