using Common;
using Common.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace TransactionCoordinator
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TransactionCoordinator : StatefulService, IStatefulInterface, ITransactionCoordinator

    {
        public TransactionCoordinator(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<string> GetServiceDetails()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Operate(List<CartItem> cart)
        {
            double sum = cart.Sum(cartItem => cartItem.Quantity * cartItem.Book.Price);
            var proxyBank = ServiceProxy.Create<IBank>(new Uri("fabric:/BankBookHub/Bank"), new ServicePartitionKey(0));
            var proxyBookstore = ServiceProxy.Create<IBookstore>(new Uri("fabric:/BankBookHub/Bookstore"), new ServicePartitionKey(0));

            var backUpBank = await proxyBank.GetAccountBalance();
            var backUpBookstore = await proxyBookstore.GetBooks();


            try
            {
                using
                    (var transaction = this.StateManager.CreateTransaction())
                {
                    if (!await proxyBookstore.HasSelectedBooks(cart)) { throw new Exception("Problem with the bookstore update."); }
                    await transaction.CommitAsync();
                }
                using (var transaction = this.StateManager.CreateTransaction())
                {
                    if (!await proxyBank.HasSufficientFunds(sum))
                    {
                        throw new Exception("Not enough funds.");
                    }
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, $"Transaction failed: {ex.Message}");
                //await proxyBank.RollbackFunds(backUpBank); 
                await proxyBookstore.RollbackBooksInventory(backUpBookstore);
                return false;
            }

            using (var transaction = this.StateManager.CreateTransaction())
            {
                if (!await proxyBank.RemoveFunds(sum))
                {
                    throw new Exception("Not enough funds.");
                }

                if (!await proxyBookstore.RemoveBooksFromStorage(cart))
                {
                    throw new Exception("Problem with the bookstore update.");
                }

                await transaction.CommitAsync();
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

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
