using System;
using System.Collections.Generic;
using System.Text;
using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Integration.Navigation;
using cloudscribe.PwaKit.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {
        public static PwaBuilder PreCacheNavigationMenuUrls(this PwaBuilder builder)
        {
            builder.Services.AddScoped<IPreCacheItemProvider, NavigationPreCacheItemProvider>();

            return builder;
        }

    }
}
