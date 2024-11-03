using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Validator
{
    public interface IValidator : IService
    {
        Task<bool> ValidateRequest(List<CartItem> cart);
    }
}
