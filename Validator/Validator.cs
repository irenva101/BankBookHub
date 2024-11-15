using Common;
using Common.Models;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric;

namespace Validator
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Validator : StatelessService, IStatelessInterface, IValidator
    {
        public Validator(StatelessServiceContext context)
            : base(context)
        {

        }

        public Task<string> GetServiceDetails()
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ValidateRequest(List<CartItem> cart)
        {
            if (cart != null)
            {
                var proxy = ServiceProxy.Create<ITransactionCoordinator>(new Uri("fabric:/BankBookHub/TransactionCoordinator"), new ServicePartitionKey(0));
                var result = await proxy.Operate(cart);

                var proxyBank = ServiceProxy.Create<IBank>(new Uri("fabric:/BankBookHub/Bank"), new ServicePartitionKey(0));
                var accountBalance = await proxyBank.GetAccountBalance();

                var proxyBookstore = ServiceProxy.Create<IBookstore>(new Uri("fabric:/BankBookHub/Bookstore"), new ServicePartitionKey(0));
                var books = await proxyBookstore.GetBooks();

                return new Result
                {
                    IsSuccess = result,
                    AccountBalance = accountBalance,
                    Books = books
                };
            }
            else
            {
                return new Result
                {
                    IsSuccess = false,
                    AccountBalance = 0,
                    Books = null
                };
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
