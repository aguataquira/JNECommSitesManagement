using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JneCommSitesManagement.Startup))]
namespace JneCommSitesManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
