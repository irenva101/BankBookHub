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
        public Bank(StatefulServiceContext context)
            : base(context)
        {
        }

        public Task<string> GetServiceDetails()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFunds(double amount)
        {
            var balances = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, double>>("balances");
            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await balances.TryGetValueAsync(tx, "accountBalance");
                if (result.HasValue && result.Value >= amount)
                {
                    await balances.AddOrUpdateAsync(tx, "accountBalance", 0, (key, value) => value - amount);
                    await tx.CommitAsync();
                    return true;
                }
                return false;
            }
        }

        public async Task<double> GetAccountBalance() { var balances = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, double>>("balances"); using (var tx = this.StateManager.CreateTransaction()) { var result = await balances.TryGetValueAsync(tx, "accountBalance"); return result.HasValue ? result.Value : 0.0; } }

        public async Task RollbackFunds(double amount)
        {
            var balances = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, double>>("balances");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await balances.AddOrUpdateAsync(tx, "accountBalance", 0.0, (key, value) => amount);
                await tx.CommitAsync();
            }
        }

        public async Task<bool> HasSufficientFunds(double amount)
        {
            var balances = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, double>>("balances");

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await balances.TryGetValueAsync(tx, "accountBalance");
                return result.HasValue && result.Value >= amount;
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
            var balances = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, double>>("balances");
            using (var tx = this.StateManager.CreateTransaction())
            {
                await balances.AddOrUpdateAsync(tx, "accountBalance", 100.0, (key, value) => value);
                await tx.CommitAsync();
            }
        }
    }
}
