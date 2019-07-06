using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cloudscribe.PwaKit.Storage.EFCore.MSSQL
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<PwaDbContext>
    {
        public PwaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PwaDbContext>();
            builder.UseSqlServer("Server=(local);Database=DATABASENAME;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new PwaDbContext(builder.Options);
        }
    }
}
