namespace AzyWorks.Pooling
{
    public abstract class GenericPool<TPoolableObject> : PoolBase
    {
        public abstract void Destroy();
        public abstract void Initialize();
        public abstract void Reset();
        public abstract void Push(TPoolableObject obj);

        public abstract bool TryGet(out TPoolableObject result);

        public abstract TPoolableObject Get();

        internal override void DestroyPool()
            => Destroy();

        internal override object GetObject()
            => Get();

        internal override void InitPool()
            => Initialize();

        internal override void PushObject(object value)
            => Push((TPoolableObject)value);

        internal override void ResetPool()
            => Reset();

        internal override bool TryGetObject(out object obj)
        {
            if (TryGet(out var result))
            {
                obj = result;
                return true;
            }

            obj = null;
            return false;
        }
    }
}
