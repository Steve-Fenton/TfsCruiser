using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TfsCruiser.Startup))]
namespace TfsCruiser
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
