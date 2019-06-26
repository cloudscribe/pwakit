using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cloudscribe.PwaKit
{
    public class PwaBuilder
    {
        public PwaBuilder(
            IServiceCollection services,
            IConfiguration config
            )
        {
            Services = services;
            Configuration = config;
        }

        public IServiceCollection Services { get; private set; }
        public IConfiguration Configuration { get; private set; }

    }
}
