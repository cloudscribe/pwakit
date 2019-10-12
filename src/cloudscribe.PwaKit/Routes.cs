﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class Routes
    {
        public static IEndpointRouteBuilder AddPwaDefaultRoutes(
            this IEndpointRouteBuilder routes,
            IRouteConstraint folderRouteConstraint = null
            )
        {
            if(folderRouteConstraint != null)
            {
                routes.MapControllerRoute(
                name: "folder-serviceworker",
                pattern: "{sitefolder}/serviceworker"
                , defaults: new { controller = "Pwa", action = "ServiceWorker" }
                , constraints: new { sitefolder = folderRouteConstraint }
                );

                routes.MapControllerRoute(
                    name: "folder-offlinepage",
                    pattern: "{sitefolder}/offline"
                    , defaults: new { controller = "Pwa", action = "Offline" }
                    , constraints: new { sitefolder = folderRouteConstraint }
                    );


            }

            routes.MapControllerRoute(
                name: "serviceworker",
                pattern: "serviceworker"
                , defaults: new { controller = "Pwa", action = "ServiceWorker" }
                );

            routes.MapControllerRoute(
                name: "offlinepage",
                pattern: "offline"
                , defaults: new { controller = "Pwa", action = "Offline" }
                );




            return routes;
        }

      }
}
