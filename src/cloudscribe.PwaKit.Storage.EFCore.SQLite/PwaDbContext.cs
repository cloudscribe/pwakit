using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Storage.EFCore.Common;
using Microsoft.EntityFrameworkCore;

namespace cloudscribe.PwaKit.Storage.EFCore.SQLite
{
    public class PwaDbContext : PwaDbContextBase, IPwaDbContext
    {
        public PwaDbContext(DbContextOptions<PwaDbContext> options) : base(options)
        {

        }

        protected PwaDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PushDeviceSubscription>(entity =>
            {
                entity.ToTable("cspwa_PushSubscription");

                entity.HasKey(p => p.Key);
                entity.Property(p => p.Key);

                entity.Property(p => p.TenantId).HasMaxLength(50);
                entity.HasIndex(p => p.TenantId);

                entity.Property(p => p.UserId).HasMaxLength(50);
                entity.HasIndex(p => p.UserId);

                entity.HasIndex(p => p.Endpoint);

                entity.Ignore(p => p.Keys);


            });

        }

    }
}
