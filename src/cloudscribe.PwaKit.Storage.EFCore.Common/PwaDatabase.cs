using cloudscribe.PwaKit.Storage.EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Hosting // so it will show up in Program without a using
{
    public static class PwaDatabase
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider scopedServiceProvider)
        {

            var db = scopedServiceProvider.GetService<IPwaDbContext>();
            await db.Database.MigrateAsync();

        }

    }
}
