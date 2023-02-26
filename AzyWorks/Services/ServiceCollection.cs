using AzyWorks.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace AzyWorks.Services
{
    public class ServiceCollection : ServiceCollectionBase
    {
        private HashSet<ServiceBase> _knownServices = new HashSet<ServiceBase>();

        public event Action<ServiceBase> OnServiceInserted;
        public event Action<ServiceBase> OnServiceDestroyed;

        public override ServiceCollectionBase RemoveService<T>()
        {
            foreach (var service in _knownServices)
            {
                if (service is T)
                {
                    service.Destroy();
                    service.Collection = null;

                    OnServiceDestroyed?.Invoke(service);
                }
            }

            _knownServices.RemoveWhere(x => x is T);
            return this;
        }

        public override T GetService<T>()
        {
            var serviceInstance = _knownServices.FirstOrDefault(x => x is T);

            if (serviceInstance is null)
                return default;

            if (!(serviceInstance is T t))
                return default;

            return t;
        }

        public override ServiceCollectionBase AddService<T>(params object[] args)
        {
            var instance = ReflectUtils.Instantiate<ServiceBase>(typeof(T));

            if (instance != null)
            {
                instance.Collection = this;
                instance.Setup(args);

                OnServiceInserted?.Invoke(instance);

                _knownServices.Add(instance);
            }

            return this;
        }

        public override T CreateService<T>(params object[] args)
        {
            var instance = ReflectUtils.Instantiate<ServiceBase>(typeof(T));

            if (instance != null)
            {
                instance.Setup(args);
                return (T) instance;
            }

            return null;
        }
    }
}
