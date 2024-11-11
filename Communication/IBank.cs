using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IBank : IService
    {
        Task<bool> RemoveFunds(double amount);
        Task<bool> HasSufficientFunds(double amount);
        Task AddFunds(double amount);
    }
}
