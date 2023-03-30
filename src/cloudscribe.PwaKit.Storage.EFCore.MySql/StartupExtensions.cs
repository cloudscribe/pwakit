using cloudscribe.PwaKit.Storage.EFCore.Common;
using cloudscribe.PwaKit.Storage.EFCore.MySql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPwaStorageMySql(
            this IServiceCollection services,
            string connectionString,
            bool useSingletonLifetime = false,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            ICollection<int> transientSqlErrorNumbersToAdd = null
            )
        {
            services.AddPwaStorageEFCommon();

            services //.AddEntityFrameworkMySql()
                .AddDbContext<PwaDbContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), // breaking change in Net5.0
                    mySqlOptionsAction: sqlOptions =>
                    {
                        if (maxConnectionRetryCount > 0)
                        {
                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: maxConnectionRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(maxConnectionRetryDelaySeconds),
                                errorNumbersToAdd: transientSqlErrorNumbersToAdd);
                        }
                    }),
                    optionsLifetime: ServiceLifetime.Singleton
                    );

            services.AddScoped<IPwaDbContext, PwaDbContext>();
            services.AddSingleton<IPwaDbContextFactory, PwaDbContextFactory>();
            

            return services;
        }

    }
}
