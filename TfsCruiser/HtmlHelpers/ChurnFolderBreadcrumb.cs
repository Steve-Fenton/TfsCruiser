namespace System.Web.Mvc.Html
{
    using System.Text;

    public static class ChurnFolderBreadcrumb
    {
        public static IHtmlString GetChurnFolderPath(this HtmlHelper htmlHelper, string path, DateTime from, DateTime to)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return new HtmlString(string.Empty);
            }

            var parts = path.Split('\\');

            var output = new StringBuilder();
            var currentPath = string.Empty;

            for (var i = 0; i < parts.Length; i++)
            {
                var priorPath = currentPath;

                if (i > 0)
                {
                    output.Append("\\");
                    currentPath += "\\";
                }

                currentPath += parts[i];

                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                var url = urlHelper.Action("Index", new { From = from, To = to, Path = priorPath, FolderDepth = i + 1, SelectedPath = currentPath });

                output.Append($"<a href=\"?{url}\">{parts[i]}</a>");
            }

            return new HtmlString(output.ToString());
        }
    }
}