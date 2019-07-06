using cloudscribe.PwaKit.Storage.EFCore.Common;
using Microsoft.EntityFrameworkCore;

namespace cloudscribe.PwaKit.Storage.EFCore.PostgreSql
{
    public class PwaDbContextFactory : IPwaDbContextFactory
    {
        public PwaDbContextFactory(DbContextOptions<PwaDbContext> options)
        {
            _options = options;
        }

        private readonly DbContextOptions<PwaDbContext> _options;

        public IPwaDbContext CreateContext()
        {
            return new PwaDbContext(_options);
        }

    }
}
