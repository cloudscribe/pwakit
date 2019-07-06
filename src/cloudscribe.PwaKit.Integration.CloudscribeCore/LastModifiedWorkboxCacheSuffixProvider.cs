using cloudscribe.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using cloudscribe.Web.Navigation;
using System.Threading.Tasks;
using System.Linq;
using cloudscribe.PwaKit.Interfaces;

namespace cloudscribe.PwaKit.Integration.CloudscribeCore
{
    public class LastModifiedWorkboxCacheSuffixProvider : IWorkboxCacheSuffixProvider
    {
        public LastModifiedWorkboxCacheSuffixProvider(
            SiteContext siteContext
            //NavigationTreeBuilderService siteMapTreeBuilder
            )
        {
            _siteContext = siteContext;
            //_siteMapTreeBuilder = siteMapTreeBuilder;
        }

        private readonly SiteContext _siteContext;
        //private readonly NavigationTreeBuilderService _siteMapTreeBuilder;

        public Task<string> GetWorkboxCacheSuffix()
        {

            //var rootNode = await _siteMapTreeBuilder.GetTree();
            //var maxContentDate = rootNode.Flatten().Where(x => x.LastModifiedUtc.HasValue).Max(x => x.LastModifiedUtc.Value);
            //if (maxContentDate > _siteContext.LastModifiedUtc)
            //{
            //    return maxContentDate.ToString("s");
            //}

            var result = _siteContext.LastModifiedUtc.ToString("s");
            return Task.FromResult(result);

        }

    }
}
