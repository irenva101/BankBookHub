using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface ITransactionCoordinator : IService
    {
        Task<bool> Operate(List<CartItem> cart);
    }
}
