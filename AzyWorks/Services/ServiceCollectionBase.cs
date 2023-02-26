namespace AzyWorks.Services
{
    public class ServiceCollectionBase
    {
        public virtual T GetService<T>() where T : ServiceBase { return null; }
        public virtual T CreateService<T>(params object[] args) where T : ServiceBase { return null; }

        public virtual ServiceCollectionBase AddService<T>(params object[] args) where T : ServiceBase { return this; }
        public virtual ServiceCollectionBase RemoveService<T>() where T : ServiceBase { return this; }
    }
}