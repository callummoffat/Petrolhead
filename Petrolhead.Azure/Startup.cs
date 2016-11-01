using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Petrolhead.Azure.Startup))]

namespace Petrolhead.Azure
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}