using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IStatelessInterface : IService
    {
        Task<string> GetServiceDetails();
    }
}
