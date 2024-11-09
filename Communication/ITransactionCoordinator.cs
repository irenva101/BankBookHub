using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface ITransactionCoordinator : IService
    {
        public Task<bool> Operate(List<CartItem> cart);
    }
}
