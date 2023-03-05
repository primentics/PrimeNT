using AzyWorks.Pooling.Pools;
using AzyWorks.System;

using System;
using System.Collections.Generic;
using System.Text;

namespace AzyWorks.Pooling
{
    public static class PoolManager
    {
        internal static Dictionary<Type, PoolBase> _registeredPools = new Dictionary<Type, PoolBase>();

        public static event Action<PoolBase> OnPoolCreated;
        public static event Action<PoolBase> OnPoolDestroyed;

        static PoolManager()
        {
            SetPool<StringBuilder>(new StringBuilderPool());
        }

        public static TPoolObject Get<TPoolObject>()
        {
            if (!TryGetPool<TPoolObject>(out var pool))
                return default;

            return Reflection.As<TPoolObject>(pool.GetObject());
        }

        public static bool TryGet<TPoolObject>(out TPoolObject poolObject)
        {
            if (!TryGetPool<TPoolObject>(out var pool))
            {
                poolObject = default;
                return false;
            }

            if (pool.TryGetObject(out var result))
            {
                poolObject = Reflection.As<TPoolObject>(result);
                return true;
            }
            else
            {
                poolObject = default;
                return false;
            }
        }

        public static bool Return<TPoolObject>(TPoolObject poolObject)
        {
            if (TryGetPool<TPoolObject>(out var pool))
            {
                pool.PushObject(poolObject);
                return true;
            }

            return false;
        }

        public static bool RemovePool<TPool>() where TPool : PoolBase
        {
            if (_registeredPools.TryGetValue(typeof(TPool), out var pool))
            {
                pool.DestroyPool();
            }
            else
            {
                return false;
            }

            if (_registeredPools.Remove(typeof(TPool)))
            {
                OnPoolDestroyed?.Invoke(pool);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void SetPool<TPoolableObject>(PoolBase pool)
        {
            _registeredPools[typeof(TPoolableObject)] = pool;

            pool.InitPool();

            OnPoolCreated?.Invoke(pool);
        }

        public static PoolBase GetPool<TPoolableObject>()
        {
            if (!_registeredPools.TryGetValue(typeof(TPoolableObject), out var pool))
                throw new KeyNotFoundException(nameof(TPoolableObject));

            return pool;
        }

        public static bool TryGetPool<TPoolableObject>(out PoolBase pool)
        {
            if (!_registeredPools.TryGetValue(typeof(TPoolableObject), out pool))
            {
                pool = default;
                return false;
            }

            return true;
        }
    }
}