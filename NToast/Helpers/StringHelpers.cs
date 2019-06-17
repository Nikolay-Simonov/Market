using Microsoft.AspNetCore.Html;

namespace NToastNotify.Helpers
{
    public static class StringHelpers
    {
        public static HtmlString ToHtmlString(this string str)
        {
            return new HtmlString(str);
        }
    }
}
