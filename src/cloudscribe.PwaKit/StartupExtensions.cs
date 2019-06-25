﻿using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {

        public static IServiceCollection AddPwaKit(
            this IServiceCollection services,
            IConfiguration config 
            )
        {

            services.Configure<PwaOptions>(config.GetSection("PwaOptions"));
            services.Configure<PwaPreCacheItems>(config.GetSection("PwaPreCacheItems"));

            services.AddScoped<IPreCacheItemProvider, ConfigPreCacheItemProvider>();
            services.AddScoped<IPreCacheItemProvider, OfflinePageCacheItemProvider>();

            services.TryAddScoped<IServiceWorkerBuilder, ServiceWorkerBuilder>();

            services.TryAddScoped<IPwaRouteNameProvider, DefaultPwaRouteNameProvider>();
            services.TryAddScoped<IOfflinePageUrlProvider, OfflinePageUrlProvider>();
            services.TryAddScoped<IWorkboxCacheSuffixProvider, ConfigWorkboxCacheSuffixProvider>();

            services.TryAddScoped<IConfigureServiceWorkerReloading, DefaultConfigureServiceWorkerReloading>();
            services.TryAddScoped<IConfigureWorkboxPreCache, DefaultConfigureWorkboxPreCache>();
            services.TryAddScoped<IConfigureWorkboxNetworkOnlyRoutes, DefaultConfigureWorkboxNetworkOnlyRoutes>();
            services.TryAddScoped<IConfigureWorkboxCacheFirstRoutes, DefaultConfigureWorkboxCacheFirstRoutes>();
            services.TryAddScoped<IConfigureWorkboxNetworkFirstRoutes, DefaultConfigureWorkboxNetworkFirstRoutes>();
            services.TryAddScoped<IConfigureWorkboxCatchHandler, DefaultConfigureWorkboxCatchHandler>();
            services.TryAddScoped<IGenerateServiceWorkerInitScript, DefaultGenerateServiceWorkerInitScript>();
            services.TryAddScoped<IConfigureWorkboxOfflineGoogleAnalytics, DefaultConfigureWorkboxOfflineGoogleAnalytics>();


            services.AddTransient<ITagHelperComponent, ServiceWorkerTagHelperComponent>();


            return services;
        }

    }
}
