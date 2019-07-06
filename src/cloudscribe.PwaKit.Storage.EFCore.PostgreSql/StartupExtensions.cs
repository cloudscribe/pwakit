using cloudscribe.PwaKit.Storage.EFCore.Common;
using cloudscribe.PwaKit.Storage.EFCore.PostgreSql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddPwaStoragePostgreSql(
            this IServiceCollection services,
            string connectionString,
            bool useSingletonLifetime = false,
            int maxConnectionRetryCount = 0,
            int maxConnectionRetryDelaySeconds = 30,
            ICollection<string> transientErrorCodesToAdd = null
            )
        {
            services.AddPwaStorageEFCommon();

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<PwaDbContext>(options =>
                    options.UseNpgsql(connectionString,
                    npgsqlOptionsAction: sqlOptions =>
                    {
                        if (maxConnectionRetryCount > 0)
                        {
                            //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: maxConnectionRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(maxConnectionRetryDelaySeconds),
                                errorCodesToAdd: transientErrorCodesToAdd);
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
