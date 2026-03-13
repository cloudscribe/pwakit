using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace cloudscribe.PwaKit
{
    internal class ServiceWorkerTagHelperComponent : TagHelperComponent
    {
        
        private IWebHostEnvironment _env;
        private IHttpContextAccessor _contextAccessor;
        private PwaOptions _options;
        private IUrlHelperFactory _urlHelperFactory;

        public ServiceWorkerTagHelperComponent(
            IUrlHelperFactory urlHelperFactory,
            IWebHostEnvironment env, 
            IHttpContextAccessor accessor, 
            IOptions<PwaOptions> pwaOptionsAccessor
            )
        {
            _urlHelperFactory = urlHelperFactory;
            _env = env;
            _contextAccessor = accessor;
            _options = pwaOptionsAccessor.Value;

        }

        /// <inheritdoc />
        public override int Order => -1;

        private string BuildScript()
        {
            var actionContext = new ActionContext(
                _contextAccessor.HttpContext,
                _contextAccessor.HttpContext.GetRouteData(),
                new ActionDescriptor()
            );
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
            var url = urlHelper.Action("Init", "Pwa");

            var script = "\r\n\t<script type=\"module\" " + (_options.EnableCspNonce ? PwaConstants.CspNonce : string.Empty) + " src='" + url + "'></script>";

            return script;
        }

        /// <inheritdoc />
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_options.AutoRegisterServiceWorker)
            {
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(_options.ExcludedAutoRegistrationPathsCsv))
            {
                var currentPath = _contextAccessor.HttpContext.Request.Path.ToString();
                var paths = _options.ExcludedAutoRegistrationPathsCsv.Split(',').ToList();
                if(paths.Any(x => currentPath.StartsWith(x)))
                {
                    return;
                }
                
            }

            if(_contextAccessor.HttpContext.Request.Method == "POST")
            {
                return;
            }

            

            if (string.Equals(context.TagName, "body", StringComparison.OrdinalIgnoreCase))
            {
                if ((_options.AllowHttp || _contextAccessor.HttpContext.Request.IsHttps) || _env.IsDevelopment())
                {
                    output.PostContent.AppendHtml(BuildScript());
                }
            }
        }
    }
}
