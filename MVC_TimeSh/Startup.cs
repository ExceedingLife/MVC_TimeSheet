using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_TimeSh.Startup))]
namespace MVC_TimeSh
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
