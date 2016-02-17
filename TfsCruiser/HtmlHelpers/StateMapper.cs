namespace System.Web.Mvc.Html
{
    public static class StateMapper
    {
        public static string MapState(this HtmlHelper htmlHelper, string state)
        {
            switch (state.ToLowerInvariant())
            {
                case "succeeded":
                    return "Succeeded";

                case "ontrack":
                    return "OnTrack";

                case "partiallysucceeded":
                    return "PartiallySucceeded";

                case "stopped":
                    return "Stopped";

                case "failed":
                default:
                    return "Failed";
            }
        }
    }
}