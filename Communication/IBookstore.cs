using Common.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IBookstore : IService
    {
        Task<Book> GetBookById(int bookId);
        Task<bool> ModifyQuantity(int bookId, int newQuantity);
        Task<bool> HasSelectedBooks(List<CartItem> cart);
        Task<bool> RemoveBooksFromStorage(List<CartItem> cart);
        Task<Dictionary<int, Book>> GetBooks();
        Task RollbackBooksInventory(Dictionary<int, Book> books);
    }
}
