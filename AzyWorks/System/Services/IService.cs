namespace AzyWorks.System.Services
{
    public interface IService
    {
        IServiceCollection Collection { get; set; }

        bool IsValid();

        void Start(IServiceCollection serviceCollection, params object[] initArgs);
        void Stop();
    }
}