using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IStatefulInterface : IService
    {
        Task<string> GetServiceDetails();
    }
}
