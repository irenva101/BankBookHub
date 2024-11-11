using Microsoft.ServiceFabric.Services.Remoting;

namespace Common
{
    public interface IStatefulInterface : IService
    {
        Task<string> GetServiceDetails();
        //Task<double> GetAccountBalance();
        //Task RemoveFunds(double amount);
        //Task<Dictionary<int, Book>> GetAllBooks();
        //Task<Book> GetBookById(int id);
        //Task RemoveBook(Book book);
    }
}
