using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace Bank
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Bank : StatefulService, IStatefulInterface, IBank
    {
        private double previousState = 0;
        public Bank(StatefulServiceContext context)
            : base(context)
        {
        }

        public Task<string> GetServiceDetails()
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasSufficientFunds(double amount)
        {
            if (amount > 100)
            {
                return Task.FromResult(false);
            }
            else
            {
                return Task.FromResult(true);
            }
        }

        public Task RemoveFunds(double amount)
        {
            previousState = DB.AccountBalance;

            // Pozivanje metode koja menja stanje
            bool result = DB.RemoveFunds(amount);

            // Ako operacija nije uspešna, možemo da vratimo stanje na prethodno
            if (!result)
            {
                // Opcionalno vra?anje prethodnog stanja
                DB.AccountBalance = previousState;
            }

            return Task.FromResult(result);
        }

        public Task<bool> RollbackTransaction()
        {
            DB.AccountBalance = previousState;
            return Task.FromResult(true);
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
