﻿using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.Web.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class NetworkOnlyUrlProvider : INetworkOnlyUrlProvider
    {
        public NetworkOnlyUrlProvider(
            NavigationTreeBuilderService siteMapTreeBuilder,
            IEnumerable<INavigationNodePermissionResolver> permissionResolvers,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccesor,
            IOptions<PwaNetworkOnlyUrlOptions> optionsAccessor
            )
        {
            _siteMapTreeBuilder   = siteMapTreeBuilder;
            _permissionResolvers  = permissionResolvers;
            _urlHelperFactory     = urlHelperFactory;
            _actionContextAccesor = actionContextAccesor;
            _options              = optionsAccessor.Value;

            _networkOnlyControllers = new List<string>()
            {
                "SiteAdmin",
                "RoleAdmin",
                "UserAdmin",
                "CoreData",
                "SystemLog",
                "Manage",
                "Account",
                "FileManager",
                "ContentSettings",
                "ContentHistory"

            };
        }

        private readonly NavigationTreeBuilderService                   _siteMapTreeBuilder;
        private readonly IEnumerable<INavigationNodePermissionResolver> _permissionResolvers;
        private readonly IUrlHelperFactory                              _urlHelperFactory;
        private readonly IActionContextAccessor                         _actionContextAccesor;
        private readonly PwaNetworkOnlyUrlOptions                       _options;

        private List<string> _networkOnlyControllers;


        public async Task<List<string>> GetNetworkOnlyUrls(PwaOptions options, HttpContext context)
        {
            var result = new List<string>()
            {
                "/blog/canedit",
                "/page/canedit",
                "/pwa/topnav"
            };

            // Network Only from json config
            if (_options?.Urls != null)
            {
                foreach (var url in _options.Urls)
                {
                    if(!result.Contains(url))  
                        result.Add(url);
                }
            }

            if (!context.User.Identity.IsAuthenticated)
            {
                //admin pages won't be in the menu anyway so just return what we have so far
                return result;
            }

            var rootNode = await _siteMapTreeBuilder.GetTree();

            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
            foreach (var navNode in rootNode.Flatten())
            {
                if(await WouldRenderNode(navNode))
                {
                    if (ShouldBeNetworkOnly(navNode))
                    {
                        var url = ResolveUrl(navNode, urlHelper);
                        result.Add(url);
                    }
                }
            }
            
            return result;
        }

        private string ResolveUrl(NavigationNode node, IUrlHelper urlHelper)
        {
            if (node.HideFromAnonymous) return string.Empty;

            // if url is already fully resolved just return it
            if (node.Url.StartsWith("http")) return node.Url;

            string urlToUse = string.Empty;
            if ((node.Action.Length > 0) && (node.Controller.Length > 0))
            {
                var a = node.Area == null ? "" : node.Area;
                urlToUse = urlHelper.Action(node.Action, node.Controller, new { area = a });
            }
            else if (node.NamedRoute.Length > 0)
            {
                urlToUse = urlHelper.RouteUrl(node.NamedRoute);
            }

            if (string.IsNullOrEmpty(urlToUse)) urlToUse = node.Url;

            if (urlToUse.StartsWith("http")) return urlToUse;

            return urlToUse;
        }

        private async Task<bool> WouldRenderNode(NavigationNode node)
        {
            //if (node.Controller == "Account") return false;

            TreeNode<NavigationNode> treeNode = new TreeNode<NavigationNode>(node);
            foreach (var permission in _permissionResolvers)
            {
                bool ok = await permission.ShouldAllowView(treeNode);
                if (!ok) return false;
            }

            return true;
        }

        private bool ShouldBeNetworkOnly(NavigationNode node)
        {
            var include = false;

            if (!string.IsNullOrEmpty(node.Controller) && _networkOnlyControllers.Contains(node.Controller))
            {
                include = true;
            } else if (node.Controller == "Page" && node.Action == "Tree")
            {
                include = true;
            }

            return include;
        }
    }
}
