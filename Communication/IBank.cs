using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IBank : IService
    {
        Task<bool> HasSufficientFunds(double amount);
        Task RemoveFunds(double amount);
        Task<bool> RollbackTransaction();
    }
}
