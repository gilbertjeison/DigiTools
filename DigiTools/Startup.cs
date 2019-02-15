using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DigiTools.Startup))]
namespace DigiTools
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
