using System.Web.Mvc;
using System.Web.Routing;

namespace TfsCruiser
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{project}/{build}",
                defaults: new { controller = "Builds", action = "Index", project = UrlParameter.Optional, build = UrlParameter.Optional });
        }
    }
}
