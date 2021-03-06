﻿using cloudscribe.PwaKit.Interfaces;
using cloudscribe.PwaKit.Models;
using cloudscribe.Web.Navigation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class NavigationRuntimeCacheItemProvider : IRuntimeCacheItemProvider
    {
        public NavigationRuntimeCacheItemProvider(
            NavigationTreeBuilderService siteMapTreeBuilder,
            IEnumerable<INavigationNodePermissionResolver> permissionResolvers,
            IEnumerable<INavigationNodeServiceWorkerFilter> navigationNodeServiceWorkerFilters,
            IUrlHelperFactory urlHelperFactory,
            IActionContextAccessor actionContextAccesor
            )
        {
            _siteMapTreeBuilder = siteMapTreeBuilder;
            _permissionResolvers = permissionResolvers;
            _navigationNodeServiceWorkerFilters = navigationNodeServiceWorkerFilters;
            _urlHelperFactory = urlHelperFactory;
            _actionContextAccesor = actionContextAccesor;
        }



        private readonly NavigationTreeBuilderService _siteMapTreeBuilder;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccesor;
        private readonly IEnumerable<INavigationNodePermissionResolver> _permissionResolvers;
        private readonly IEnumerable<INavigationNodeServiceWorkerFilter> _navigationNodeServiceWorkerFilters;

        private List<string> addedUrls = new List<string>();
        

        public async Task<List<ServiceWorkerCacheItem>> GetItems()
        {
            var result = new List<ServiceWorkerCacheItem>();
            var rootNode = await _siteMapTreeBuilder.GetTree();
            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccesor.ActionContext);
            foreach (var navNode in rootNode.Flatten())
            {
                bool include = true;
                foreach (var filter in _navigationNodeServiceWorkerFilters)
                {
                    include = await filter.ShouldRenderNode(navNode);
                }

                if (!include) continue;


                if (! await ShouldRenderNode(navNode)) continue;

                var url = ResolveUrl(navNode, urlHelper);

                if (string.IsNullOrWhiteSpace(url)) continue;

                if (addedUrls.Contains(url)) continue;

                

                result.Add(new ServiceWorkerCacheItem()
                {
                    Url = url,
                    LastModifiedUtc = navNode.LastModifiedUtc
                });

            }


            return result;
        }


        private async Task<bool> ShouldRenderNode(NavigationNode node)
        {
            //if (node.Controller == "Account") return false;

            // if any node fails to add to cahce then all nodes fails so exclude any nodes that have a policy or role requirement
            if (!string.IsNullOrWhiteSpace(node.AuthorizationPolicy)) return false;

            if (!string.IsNullOrWhiteSpace(node.ViewRoles)) return false;

            TreeNode<NavigationNode> treeNode = new TreeNode<NavigationNode>(node);
            foreach (var permission in _permissionResolvers)
            {
                bool ok = await permission.ShouldAllowView(treeNode);
                if (!ok) return false;
            }

            return true;
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


    }
}
