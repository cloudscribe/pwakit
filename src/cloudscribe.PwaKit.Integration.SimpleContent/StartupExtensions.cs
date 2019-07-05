using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Integration.SimpleContent;
using cloudscribe.PwaKit.Integration.SimpleContent.Handlers;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.SimpleContent.Models.EventHandlers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static PwaBuilder PreCacheAllSimpleContentUrls(this PwaBuilder builder)
        {

            builder.Services.AddScoped<IRuntimeCacheItemProvider, BlogRuntimeCacheItemProvider>();

            builder.Services.AddScoped<IHandlePageCreated, PageCreatedNotifyServiceWorkerCache>();
            builder.Services.AddScoped<IHandlePageUpdated, PageUpdatedNotifyServiceWorkerCache>();
            builder.Services.AddScoped<IHandlePagePreDelete, PageDeleteNotifyServiceWorkerCache>();

            builder.Services.AddScoped<IHandlePostCreated, PostCreatedNotifyServiceWorkerCache>();
            builder.Services.AddScoped<IHandlePostUpdated, PostUpdatedNotifyServiceWorkerCache>();
            builder.Services.AddScoped<IHandlePostPreDelete, PostDeleteNotifyServiceWorkerCache>();



            return builder;
        }

    }
}
