using cloudscribe.PwaKit.Interfaces;
using Lib.Net.Http.WebPush;
using NoDb;
using System;
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
            await _commands.DeleteAsync(_NoDbProjectId, endpoint).ConfigureAwait(false);
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

        public async Task StoreSubscriptionAsync(PushSubscription subscription)
        {
            var newSub = new cloudscribe.PwaKit.Models.PushSubscription(subscription);

            await _commands.CreateAsync(
                _NoDbProjectId,
                newSub.Endpoint,
                newSub
                ).ConfigureAwait(false);
        }
    }
}
