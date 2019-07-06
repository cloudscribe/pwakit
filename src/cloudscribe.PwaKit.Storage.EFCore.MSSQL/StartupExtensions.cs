using cloudscribe.PwaKit.Storage.EFCore.Common;
using cloudscribe.PwaKit.Storage.EFCore.MSSQL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static  class StartupExtensions
    {
        public static IServiceCollection AddPwaStorageMSSQL(
            this IServiceCollection services,
            string connectionString,
            bool useSingletonLifetime = false,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            ICollection<int> transientSqlErrorNumbersToAdd = null,
            bool useSql2008Compatibility = false
            )
        {
            services.AddPwaStorageEFCommon();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<PwaDbContext>(options =>
                    options.UseSqlServer(connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            if (maxConnectionRetryCount > 0)
                            {
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                sqlOptions.EnableRetryOnFailure(
                                    maxRetryCount: maxConnectionRetryCount,
                                    maxRetryDelay: TimeSpan.FromSeconds(maxConnectionRetryDelaySeconds),
                                    errorNumbersToAdd: transientSqlErrorNumbersToAdd);
                            }

                            if (useSql2008Compatibility)
                            {
                                sqlOptions.UseRowNumberForPaging();
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
