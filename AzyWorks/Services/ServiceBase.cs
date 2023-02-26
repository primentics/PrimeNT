namespace AzyWorks.Services
{
    public class ServiceBase
    {
        public ServiceCollectionBase Collection { get; internal set; }

        public virtual void Setup(object[] args)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}