using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Storage.EFCore.Common
{
    public class PushSubscriptionStore : IPushSubscriptionStore
    {
        public PushSubscriptionStore(IPwaDbContextFactory pwaDbContextFactory)
        {
            _contextFactory = pwaDbContextFactory;
        }

        private readonly IPwaDbContextFactory _contextFactory;

        public async Task<IEnumerable<PushDeviceSubscription>> GetAllSubscriptions(
           string tenantId,
           CancellationToken cancellationToken = default(CancellationToken)
           )
        {
            using (var db = _contextFactory.CreateContext())
            {
                var query = db.PushSubscriptions.Where(x => x.TenantId == tenantId);
                return await query.AsNoTracking().ToListAsync();

            }

        }

        public async Task<IEnumerable<PushDeviceSubscription>> GetSubscriptionsForUser(
            string tenantId,
            string userId,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            using (var db = _contextFactory.CreateContext())
            {
                var query = db.PushSubscriptions.Where(x => x.TenantId == tenantId && x.UserId == userId);
                return await query.AsNoTracking().ToListAsync();

            }
        }

        public async Task<IEnumerable<PushDeviceSubscription>> GetAllSubscriptionsExceptForUser(
           string tenantId,
           string userId,
           CancellationToken cancellationToken = default(CancellationToken)
           )
        {
            using (var db = _contextFactory.CreateContext())
            {
                var query = db.PushSubscriptions.Where(x => x.TenantId == tenantId && x.UserId != userId);
                return await query.AsNoTracking().ToListAsync();

            }
        }

        public async Task<PushDeviceSubscription> GetSubscriptionByEndpoint(
            string tenantId,
            string endpoint,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            using (var db = _contextFactory.CreateContext())
            {
                var query = db.PushSubscriptions.Where(x => x.TenantId == tenantId && x.Endpoint == endpoint);
                return await query.AsNoTracking().FirstOrDefaultAsync();

            }
        }

        public async Task CreateSubscription(PushDeviceSubscription subscription)
        {
            using (var db = _contextFactory.CreateContext())
            {
                db.PushSubscriptions.Add(subscription);

                int rowsAffected = await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task UpdateSubscription(PushDeviceSubscription subscription)
        {
            using (var db = _contextFactory.CreateContext())
            {
                bool tracking = db.ChangeTracker.Entries<PushDeviceSubscription>().Any(x => x.Entity.Key == subscription.Key);
                if (!tracking)
                {
                    db.PushSubscriptions.Update(subscription);
                }

                int rowsAffected = await db.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        public async Task DeleteSubscription(string endpoint)
        {
            using (var db = _contextFactory.CreateContext())
            {
                var itemToRemove = await db.PushSubscriptions.FirstOrDefaultAsync(x => x.Endpoint == endpoint);
                db.PushSubscriptions.Remove(itemToRemove);
                int rowsAffected = await db.SaveChangesAsync()
                    .ConfigureAwait(false);
            }
        }



    }

}
