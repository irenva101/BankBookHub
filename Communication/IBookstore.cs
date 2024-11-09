using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IBookstore : IService
    {
        Task<bool> HasSelectedBooks(List<CartItem> cart);
        Task RemoveBooksFromStorage(List<CartItem> cart);
        Task<bool> RollbackTransaction();
    }
}
