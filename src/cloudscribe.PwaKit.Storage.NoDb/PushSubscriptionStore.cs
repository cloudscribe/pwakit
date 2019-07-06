using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using NoDb;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Storage.NoDb
{
    public class PushSubscriptionStore : IPushSubscriptionStore
    {
        public PushSubscriptionStore(
            IBasicCommands<PushDeviceSubscription> commands,
            IBasicQueries<PushDeviceSubscription> queries
            )
        {
            _commands = commands;
            _queries = queries;
        }

        private readonly IBasicCommands<PushDeviceSubscription> _commands;
        private readonly IBasicQueries<PushDeviceSubscription> _queries;

        //for NoDb we are storing all in default project as we don't have per tenant queues
        private const string _NoDbProjectId = "default";

        public async Task<IEnumerable<PushDeviceSubscription>> GetAllSubscriptions(
            string tenantId,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var all = await _queries.GetAllAsync(_NoDbProjectId).ConfigureAwait(false);

            return all.Where(x => x.TenantId == tenantId);
        }


        public async Task<PushDeviceSubscription> GetSubscriptionByEndpoint(
            string tenantId,
            string endpoint,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var all = await _queries.GetAllAsync(_NoDbProjectId).ConfigureAwait(false);

            return all.Where(x => x.TenantId == tenantId && x.Endpoint == endpoint).FirstOrDefault();
        }

        public async Task<IEnumerable<PushDeviceSubscription>> GetAllSubscriptionsExceptForUser(
            string tenantId,
            string userId,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var all = await _queries.GetAllAsync(_NoDbProjectId).ConfigureAwait(false);

            return all.Where(x => x.TenantId == tenantId && x.UserId != userId);
        }

        public async Task<IEnumerable<PushDeviceSubscription>> GetSubscriptionsForUser(
            string tenantId,
            string userId,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            var all = await _queries.GetAllAsync(_NoDbProjectId).ConfigureAwait(false);

            return all.Where(x => x.TenantId == tenantId && x.UserId == userId);
        }

        public async Task DeleteSubscription(string endpoint)
        {
            var all = await _queries.GetAllAsync(_NoDbProjectId).ConfigureAwait(false);
            var found = all.Where(x => x.Endpoint == endpoint).SingleOrDefault();
            if(found != null)
            {
                await _commands.DeleteAsync(_NoDbProjectId, found.Key.ToString()).ConfigureAwait(false);
            }
            
        }
        
        public async Task CreateSubscription(PushDeviceSubscription subscription)
        {
            await _commands.CreateAsync(
                _NoDbProjectId,
                subscription.Key.ToString(),
                subscription
                ).ConfigureAwait(false);
        }

        public async Task UpdateSubscription(PushDeviceSubscription subscription)
        {
            await _commands.UpdateAsync(
                _NoDbProjectId,
                subscription.Key.ToString(),
                subscription
                ).ConfigureAwait(false);
        }

    }
}
