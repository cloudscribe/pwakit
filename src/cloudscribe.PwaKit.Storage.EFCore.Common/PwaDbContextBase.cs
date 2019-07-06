using cloudscribe.PwaKit.Models;
using Microsoft.EntityFrameworkCore;

namespace cloudscribe.PwaKit.Storage.EFCore.Common
{
    public class PwaDbContextBase : DbContext
    {
        public PwaDbContextBase(DbContextOptions options) : base(options)
        { }

        protected PwaDbContextBase()
        { }

        public DbSet<PushDeviceSubscription> PushSubscriptions { get; set; }

    }
}
