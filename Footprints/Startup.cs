using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Footprints.Startup))]
namespace Footprints
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
