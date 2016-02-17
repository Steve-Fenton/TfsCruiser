using System.Configuration;
using System.Web.Optimization;

namespace TfsCruiser
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/app").Include(
                "~/Scripts/Logging.js",
                "~/Scripts/Http.js",
                "~/Scripts/Encoding.js",
                "~/Scripts/Ajax.js",
                "~/Scripts/Notifier.js",
                "~/Scripts/app.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/style.css"));

            if (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["EnableOptimizations"]) || ConfigurationManager.AppSettings["EnableOptimizations"].Equals("true", System.StringComparison.InvariantCultureIgnoreCase))
            {
                BundleTable.EnableOptimizations = true;
            }
        }
    }
}