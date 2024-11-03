using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IStatelessInterface : IService
    {
        Task<string> GetServiceDetails();

    }
}
