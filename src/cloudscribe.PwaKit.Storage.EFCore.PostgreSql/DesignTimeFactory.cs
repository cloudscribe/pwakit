using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cloudscribe.PwaKit.Storage.EFCore.PostgreSql
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<PwaDbContext>
    {
        public PwaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PwaDbContext>();
            builder.UseNpgsql("server=yourservername;UID=yourdatabaseusername;PWD=yourdatabaseuserpassword;database=yourdatabasename");

            return new PwaDbContext(builder.Options);
        }
    }
}
