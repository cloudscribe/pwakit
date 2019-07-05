using cloudscribe.FileManager.Web.Events;
using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Integration.CloudscribeCore;
using cloudscribe.PwaKit.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        
        public static PwaBuilder AddCloudscribeCoreIntegration(this PwaBuilder builder)
        {

            builder.Services.AddScoped<IPwaRouteNameProvider, PwaRouteNameProvider>();
            //builder.Services.AddScoped<IWorkboxCacheSuffixProvider, LastModifiedWorkboxCacheSuffixProvider>();
            builder.Services.AddScoped<IUserIdResolver, UserIdResolver>();
            builder.Services.AddScoped<ITenantIdResolver, TenantIdResolver>();


            return builder;
        }

        public static PwaBuilder MakeCloudscribeAdminPagesNetworkOnly(this PwaBuilder builder)
        {

            builder.Services.AddScoped<INavigationNodeServiceWorkerFilter, AdminNodeServiceWorkerPreCacheFilter>();
            builder.Services.AddScoped<INetworkOnlyUrlProvider, NetworkOnlyUrlProvider>();

            return builder;
        }

        /// <summary>
        /// precaches urls for all image files found under site upload area
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static PwaBuilder PreCacheAllFileManagerImageUrls(this PwaBuilder builder)
        {
            builder.Services.Configure<PwaContentFilesPreCacheOptions>(builder.Configuration.GetSection("PwaContentFilesPreCacheOptions"));

            builder.Services.AddScoped<IRuntimeCacheItemProvider, ContentFilesRuntimeCacheItemProvider>();
            builder.Services.AddScoped<IHandleFilesUploaded, FileUploadCacheUpdateHandler>();


            return builder;
        }

        public static PwaBuilder PreCacheNavigationMenuUrls(this PwaBuilder builder)
        {
            builder.Services.AddScoped<IRuntimeCacheItemProvider, NavigationRuntimeCacheItemProvider>();

            return builder;
        }

    }
}
