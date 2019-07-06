using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CloudscribeFeatures
    {

        public static IServiceCollection SetupDataStorage(
            this IServiceCollection services,
            IConfiguration config,
            bool useHangfire
            )
        {


            var storage = config["DevOptions:DbPlatform"];
            var efProvider = config["DevOptions:EFProvider"];


            switch (storage)
            {
                case "NoDb":

                    var useSingletons = true;
                    services.AddCloudscribeCoreNoDbStorage(useSingletons);
                    services.AddNoDbStorageForSimpleContent(useSingletons);
                    services.AddCloudscribeLoggingNoDbStorage(config, useSingletons);

                    services.AddPwaNoDbStorage();

                    break;

                case "ef":
                default:

                    switch (efProvider)
                    {
                        case "sqlite":
                            var slConnection = config.GetConnectionString("SQLiteEntityFrameworkConnectionString");
                            services.AddCloudscribeCoreEFStorageSQLite(slConnection);
                            services.AddCloudscribeSimpleContentEFStorageSQLite(slConnection);
                            services.AddCloudscribeLoggingEFStorageSQLite(slConnection);

                            services.AddPwaStorageSQLite(slConnection);
                            
                            break;
                            
                        case "pgsql":
                            var pgsConnection = config.GetConnectionString("PostgreSqlConnectionString");
                            services.AddCloudscribeCorePostgreSqlStorage(pgsConnection);
                            services.AddCloudscribeSimpleContentPostgreSqlStorage(pgsConnection);
                            services.AddCloudscribeLoggingPostgreSqlStorage(pgsConnection);

                            services.AddPwaStoragePostgreSql(pgsConnection);
                            
                            break;

                        case "MySql":
                            var mysqlConnection = config.GetConnectionString("MySqlEntityFrameworkConnectionString");
                            services.AddCloudscribeCoreEFStorageMySql(mysqlConnection);
                            services.AddCloudscribeSimpleContentEFStorageMySQL(mysqlConnection);
                            services.AddCloudscribeLoggingEFStorageMySQL(mysqlConnection);

                            services.AddPwaStorageMySql(mysqlConnection);
                            
                            break;

                        case "MSSQL":
                        default:
                            var connectionString = config.GetConnectionString("EntityFrameworkConnection");
                            services.AddCloudscribeCoreEFStorageMSSQL(connectionString);
                            services.AddCloudscribeSimpleContentEFStorageMSSQL(connectionString);
                            services.AddCloudscribeLoggingEFStorageMSSQL(connectionString);

                            services.AddPwaStorageMSSQL(connectionString);
                            
                            break;
                    }


                    break;
            }
            
            return services;
        }

        public static IServiceCollection SetupCloudscribeFeatures(
            this IServiceCollection services,
            IConfiguration config
            )
        {

            services.AddCloudscribeLogging(config);
            
            services.AddScoped<cloudscribe.Web.Navigation.INavigationNodePermissionResolver, cloudscribe.Web.Navigation.NavigationNodePermissionResolver>();
            services.AddScoped<cloudscribe.Web.Navigation.INavigationNodePermissionResolver, cloudscribe.SimpleContent.Web.Services.PagesNavigationNodePermissionResolver>();
            services.AddCloudscribeCoreMvc(config);
            services.AddCloudscribeCoreIntegrationForSimpleContent(config);
            services.AddSimpleContentMvc(config);
            services.AddContentTemplatesForSimpleContent(config);

            services.AddMetaWeblogForSimpleContent(config.GetSection("MetaWeblogApiOptions"));
            services.AddSimpleContentRssSyndiction();


            var pwaBuilder = services.AddPwaKit(config);
            pwaBuilder.AddCloudscribeCoreIntegration();

            pwaBuilder.UseSiteLastModifiedAsCacheSuffix();
            pwaBuilder.MakeCloudscribeAdminPagesNetworkOnly();
            pwaBuilder.PreCacheAllFileManagerImageUrls();
            pwaBuilder.PreCacheNavigationMenuUrls();
            pwaBuilder.PreCacheAllSimpleContentUrls();
            
            

            return services;
        }

    }
}
