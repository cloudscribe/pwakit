using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Integration.SimpleContent;
using cloudscribe.PwaKit.Integration.SimpleContent.Handlers;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.SimpleContent.Models.EventHandlers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static PwaBuilder PreCacheAllBlogPostUrls(this PwaBuilder builder)
        {

            builder.Services.AddScoped<IPreCacheItemProvider, BlogPreCacheItemProvider>();

            builder.Services.AddScoped<IHandlePageUpdated, PageUpdatedNotifyServiceWorkerCache > ();


            return builder;
        }

    }
}
