using Microsoft.ServiceFabric.Services.Remoting;

namespace Bank
{
    public interface IBank : IService
    {
        Task<bool> HasSufficientFunds(double amount);
    }
}
