using Common;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace Bookstore
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Bookstore : StatefulService, IStatefulInterface, IBookstore
    {
        public Bookstore(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<bool> ModifyQuantity(int bookId, int newQuantity)
        {
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Book>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                if (await books.ContainsKeyAsync(tx, bookId))
                {
                    await books.AddOrUpdateAsync(tx, bookId, default(Book), (key, value) =>
                    { value.Quantity = newQuantity; return value; });
                    await tx.CommitAsync();

                    return true;
                }
                return false;
            }
        }
        public async Task RollbackBooksInventory(List<CartItem> cart)
        {
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Book>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                foreach (var item in cart)
                {
                    await books.AddOrUpdateAsync(tx, item.Book.Id, default(Book), (key, value) => { value.Quantity += item.Quantity; return value; });
                }
                await tx.CommitAsync();
            }
        }


        public async Task<Book> GetBookById(int bookId)
        {
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Book>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await books.TryGetValueAsync(tx, bookId);
                return result.HasValue ? result.Value : null;
            }
        }

        public Task<string> GetServiceDetails()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> HasSelectedBooks(List<CartItem> cart)
        {
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Book>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                foreach (var item in cart)
                {
                    var result = await books.TryGetValueAsync(tx, item.Book.Id);
                    if (!result.HasValue || result.Value.Quantity < item.Quantity)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public async Task<bool> RemoveBooksFromStorage(List<CartItem> cart)
        {
            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Book>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                foreach (var item in cart)
                {
                    var result = await books.TryGetValueAsync(tx, item.Book.Id);
                    if (result.HasValue && result.Value.Quantity >= item.Quantity)
                    {
                        await books.AddOrUpdateAsync(tx, item.Book.Id, default(Book), (key, value) =>
                        {
                            value.Quantity -= item.Quantity;
                            return value;
                        });
                    }
                    else
                    {
                        return false;
                    }
                }
                await tx.CommitAsync();
                return true;
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var books = await this.StateManager.GetOrAddAsync<IReliableDictionary<int, Book>>("books");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await books.AddOrUpdateAsync(tx, 1, new Book { Id = 1, Title = "The Name of the Wind", Price = 19.99, Quantity = 3, Description = "The Name of the Wind follows the journey of Kvothe, a gifted young man who grows up to become a legendary figure." }, (key, value) => value);
                await books.AddOrUpdateAsync(tx, 2, new Book { Id = 2, Title = "Rebecca", Quantity = 6, Price = 24.99, Description = "Rebecca tells the story of a young, unnamed protagonist who marries the wealthy widower Maxim de Winter and moves to his grand estate, Manderley." }, (key, value) => value);
                await books.AddOrUpdateAsync(tx, 3, new Book { Id = 3, Title = "1984", Quantity = 5, Price = 10.50, Description = "1984 is a dystopian novel set in a totalitarian regime governed by the Party, led by the figurehead Big Brother.", }, (key, value) => value);
                await tx.CommitAsync();
            }
        }
    }
}
