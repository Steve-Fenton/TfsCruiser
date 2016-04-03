using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(TfsCruiser.Startup))]

namespace TfsCruiser
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}