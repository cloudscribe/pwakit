using cloudscribe.PwaKit.Storage.EFCore.Common;
using cloudscribe.PwaKit.Storage.EFCore.SQLite;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPwaStorageSQLite(
            this IServiceCollection services,
            string connectionString
            )
        {
            services.AddPwaStorageEFCommon();

            services.AddDbContext<PwaDbContext>(options =>
                    options.UseSqlite(connectionString), 
                    optionsLifetime: ServiceLifetime.Singleton
                    );

            services.AddScoped<IPwaDbContext, PwaDbContext>();
            services.AddSingleton<IPwaDbContextFactory, PwaDbContextFactory>();

            

            return services;
        }

    }
}
