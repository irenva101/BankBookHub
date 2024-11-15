using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Validator
{
    public interface IValidator : IService
    {
        Task<Result> ValidateRequest(List<CartItem> cart);
    }
}
