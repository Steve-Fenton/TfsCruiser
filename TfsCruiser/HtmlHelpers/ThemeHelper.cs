using System.Configuration;

namespace System.Web.Mvc.Html
{
    public static class ThemeHelper
    {
        public static string Theme(this HtmlHelper htmlHelper)
        {
            return ConfigurationManager.AppSettings["Theme"] ?? "default";
        }
    }
}