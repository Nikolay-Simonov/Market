using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Text;


namespace Market.BLL.Extensions
{
    public static class IdentityResultExtensions
    {
        public static string BuildMessage(this IdentityResult identityResult,
            string fallbackMessage = "An error has occurred.")
        {
            if (identityResult.Errors == null || !identityResult.Errors.Any())
            {
                return fallbackMessage;
            }

            var stringBuilder = new StringBuilder();

            foreach (var msg in identityResult.Errors)
            {
                if (!string.IsNullOrWhiteSpace(msg.Description))
                {
                    stringBuilder.AppendLine(msg.Description);
                }
            }

            string result = stringBuilder.ToString();

            return string.IsNullOrWhiteSpace(result)
                ? fallbackMessage
                : result;
        }
    }
}