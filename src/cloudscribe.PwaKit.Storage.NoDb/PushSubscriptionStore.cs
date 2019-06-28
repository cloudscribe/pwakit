using cloudscribe.PwaKit.Interfaces;
using Lib.Net.Http.WebPush;
using NoDb;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Storage.NoDb
{
    public class PushSubscriptionStore : IPushSubscriptionStore
    {
        public PushSubscriptionStore(
            IBasicCommands<cloudscribe.PwaKit.Models.PushSubscription> commands,
            IBasicQueries<cloudscribe.PwaKit.Models.PushSubscription> queries
            )
        {
            _commands = commands;
            _queries = queries;
        }

        private readonly IBasicCommands<cloudscribe.PwaKit.Models.PushSubscription> _commands;
        private readonly IBasicQueries<cloudscribe.PwaKit.Models.PushSubscription> _queries;

        private const string _NoDbProjectId = "default";

        public async Task DiscardSubscriptionAsync(string endpoint)
        {
            var all = await _queries.GetAllAsync(_NoDbProjectId).ConfigureAwait(false);
            var found = all.Where(x => x.Endpoint == endpoint).SingleOrDefault();
            if(found != null)
            {
                await _commands.DeleteAsync(_NoDbProjectId, found.Key.ToString()).ConfigureAwait(false);
            }
            
        }

        public async Task ForEachSubscriptionAsync(Action<PushSubscription> action)
        {
           await ForEachSubscriptionAsync(action, CancellationToken.None);
        }

        public async Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var all = await _queries.GetAllAsync(_NoDbProjectId, cancellationToken).ConfigureAwait(false);
            foreach(var item in all)
            {
                action(item);
            }
            
        }

        public async Task StoreSubscriptionAsync(cloudscribe.PwaKit.Models.PushSubscription subscription)
        {
            //var newSub = new cloudscribe.PwaKit.Models.PushSubscription(subscription);

            await _commands.CreateAsync(
                _NoDbProjectId,
                subscription.Key.ToString(),
                subscription
                ).ConfigureAwait(false);
        }
    }
}
