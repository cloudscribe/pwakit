﻿using cloudscribe.PwaKit;
using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.PwaKit.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class StartupExtensions
    {

        public static PwaBuilder AddPwaKit(
            this IServiceCollection services,
            IConfiguration config 
            )
        {
            var builder = new PwaBuilder(services, config);

            services.Configure<PwaOptions>(config.GetSection("PwaOptions"));
            services.Configure<PwaPreCacheItems>(config.GetSection("PwaPreCacheItems"));
            services.Configure<PwaRuntimeCacheItems>(config.GetSection("PwaRuntimeCacheItems"));
            services.Configure<PwaNetworkOnlyUrlOptions>(config.GetSection("PwaNetworkOnlyUrlOptions"));


            services.Configure<PushClientSettings>(config.GetSection("PushServiceClient"));

            
            services.AddScoped<IRuntimeCacheItemProvider, ConfigRuntimeCacheItemProvider>();


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
            services.TryAddScoped<IGeneratePwaInitScript, DefaultGeneratePwaInitScript>();
            services.TryAddScoped<IConfigureWorkboxOfflineGoogleAnalytics, DefaultConfigureWorkboxOfflineGoogleAnalytics>();
            services.TryAddScoped<IConfigureServiceWorkerPushNotification, DefaultConfigureServiceWorkerPushNotification>();

            services.TryAddScoped<IConfigurePushApiMethods, DefaultConfigurePushApiMethods>();

            services.TryAddScoped<IUserIdResolver, DefaultUserIdResolver>();
            services.TryAddScoped<ITenantIdResolver, DefaultTenantIdResolver>();


            services.AddTransient<ITagHelperComponent, ServiceWorkerTagHelperComponent>();



            services.AddMemoryCache();
            services.AddMemoryVapidTokenCache();
            services.AddPushServiceClient(options =>
            {
                IConfigurationSection pushNotificationServiceConfigurationSection = config.GetSection(nameof(PushServiceClient));

                options.Subject = pushNotificationServiceConfigurationSection.GetValue<string>(nameof(options.Subject));
                options.PublicKey = pushNotificationServiceConfigurationSection.GetValue<string>(nameof(options.PublicKey));
                options.PrivateKey = pushNotificationServiceConfigurationSection.GetValue<string>(nameof(options.PrivateKey));
            });
            services.AddTransient<IPushNotificationService, PushServicePushNotificationService>();

            services.AddSingleton<IPushNotificationsQueue, PushNotificationQueue>();
            services.AddSingleton<IHostedService, PushNotificationBackgroundTask>();

            services.AddSingleton<IPushNotificationRecipientProvider, AllSubscribersPushNotificationRecipientProvider>();
            services.AddSingleton<IPushNotificationRecipientProvider, AllButCurrentUserPushNotificationRecipientProvider>();
            services.AddSingleton<IPushNotificationRecipientProvider, SingleUserPushNotificationRecipientProvider>();


            return builder;
        }

        

    }
}
