using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace Market.BLL.Extensions
{
    public static class AppBuilderExntensions
    {
        /// <summary>
        /// Изменяет разделители для чисел с плавающей точкой.
        /// </summary>
        public static void ChangeCurrentLocale(this IApplicationBuilder app)
        {
            const string defaultDateCulture = "ru-RU";
            var ci = new CultureInfo(defaultDateCulture)
            {
                NumberFormat =
                {
                    NumberDecimalSeparator = ".",
                    CurrencyDecimalSeparator = "."
                }
            };

            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo>
                {
                    ci
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    ci
                }
            });
        }
    }
}