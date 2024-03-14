using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TicketSystem.Startup))]
namespace TicketSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
