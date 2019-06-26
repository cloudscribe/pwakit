using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Integration.SimpleContent;
using cloudscribe.PwaKit.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static PwaBuilder PreCacheAllBlogPostUrls(this PwaBuilder builder)
        {

            builder.Services.AddScoped<IPreCacheItemProvider, BlogPreCacheItemProvider>();


            return builder;
        }

    }
}
