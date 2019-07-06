using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace cloudscribe.PwaKit.Storage.EFCore.MySql
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<PwaDbContext>
    {
        public PwaDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PwaDbContext>();
            builder.UseMySql("Server=yourserver;Database=yourdb;Uid=youruser;Pwd=yourpassword;Charset=utf8;");

            return new PwaDbContext(builder.Options);
        }
    }
}
