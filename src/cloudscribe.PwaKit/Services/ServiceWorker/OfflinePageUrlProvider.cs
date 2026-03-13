using cloudscribe.PwaKit.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace cloudscribe.PwaKit.Services
{
    public class OfflinePageUrlProvider : IOfflinePageUrlProvider
    {
        public OfflinePageUrlProvider(
            IUrlHelperFactory urlHelperFactory,
            IHttpContextAccessor contextAccessor,
            IPwaRouteNameProvider pwaRouteNameProvider
            )
        {
            _urlHelperFactory = urlHelperFactory;
            _contextAccessor = contextAccessor;
            _pwaRouteNameProvider = pwaRouteNameProvider;
        }

        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPwaRouteNameProvider _pwaRouteNameProvider;

        public string GetOfflineUrl()
        {
            var actionContext = new ActionContext(
                _contextAccessor.HttpContext,
                _contextAccessor.HttpContext.GetRouteData(),
                new ActionDescriptor()
            );
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            return urlHelper.RouteUrl(_pwaRouteNameProvider.GetOfflinePageRouteName());

        }


    }
}
