﻿using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TfsCruiser
{
#pragma warning disable SA1649 // File name must match first type name
    public class MvcApplication : System.Web.HttpApplication
#pragma warning restore SA1649 // File name must match first type name
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}