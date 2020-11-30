using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NET_Phones2020.Startup))]
namespace NET_Phones2020
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
