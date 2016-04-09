namespace System.Web.Mvc.Html
{
    using System.Configuration;
    using Fenton.Rest;

    public static class ThemeHelper
    {
        public static string Theme(this HtmlHelper htmlHelper)
        {
            return Config.GetType("Theme", "default");
        }
    }
}