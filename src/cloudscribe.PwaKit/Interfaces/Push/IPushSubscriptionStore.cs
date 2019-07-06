using cloudscribe.PwaKit.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Interfaces
{
    public interface IPushSubscriptionStore
    {

        Task<IEnumerable<PushDeviceSubscription>> GetAllSubscriptions(
            string tenantId,
            CancellationToken cancellationToken = default(CancellationToken)
            );

        Task<IEnumerable<PushDeviceSubscription>> GetAllSubscriptionsExceptForUser(
           string tenantId,
           string userId,
           CancellationToken cancellationToken = default(CancellationToken)
           );

        Task<PushDeviceSubscription> GetSubscriptionByEndpoint(
            string tenantId,
            string endpoint,
            CancellationToken cancellationToken = default(CancellationToken)
            );

        Task<IEnumerable<PushDeviceSubscription>> GetSubscriptionsForUser(
            string tenantId,
            string userId,
            CancellationToken cancellationToken = default(CancellationToken)
            );

        Task CreateSubscription(PushDeviceSubscription subscription);

        Task UpdateSubscription(PushDeviceSubscription subscription);

        Task DeleteSubscription(string endpoint);

        

    }
}
