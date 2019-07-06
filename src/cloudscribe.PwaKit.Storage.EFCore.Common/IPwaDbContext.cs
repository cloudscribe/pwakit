using cloudscribe.PwaKit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Storage.EFCore.Common
{
    public interface IPwaDbContext : IDisposable
    {
        DbSet<PushDeviceSubscription> PushSubscriptions { get; set; }

        ChangeTracker ChangeTracker { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

    }
}
