using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cloudscribe.PwaKit.Storage.EFCore.SQLite
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<PwaDbContext>
    {
        public PwaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PwaDbContext>();
            builder.UseSqlite("Data Source=cloudscribe.db");

            return new PwaDbContext(builder.Options);
        }
    }
}
