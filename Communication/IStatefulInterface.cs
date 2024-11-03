using Microsoft.ServiceFabric.Services.Remoting;

namespace Communication
{
    public interface IStatefulInterface : IService
    {
        Task<string> GetServiceDetails();
    }
}
