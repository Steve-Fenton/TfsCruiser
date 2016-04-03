namespace System.Web.Mvc.Html
{
    public static class DynamicLayoutHelper
    {
        public static string TileHeight(this HtmlHelper htmlHelper, int tileCount, int columns, decimal gapPercentage = 0, int precision = 6)
        {
            var tilesPerColumn = Math.Ceiling((decimal)tileCount / columns);

            // var tileHeight = Math.Round(((100 / tilesPerColumn) - gapPercentage), precision);
            return $"calc((100vh - 33.5px) / {tilesPerColumn})";
        }

        public static string MiniTileWidth(this HtmlHelper htmlHelper, int tileCount, int precision = 6)
        {
            var tileWidth = Math.Round((decimal)(100 / (tileCount + 1)), 6);
            return $"calc(100% / {tileCount})";
        }
    }
}