using System;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Extensions
{
    internal static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddJson(this IMvcBuilder builder)
        {
            if (builder?.Services == null)
            {
                throw new ArgumentNullException(nameof(builder),
                    "The \"builder\" and \"builder.Service\" can't be null.");
            }

            // Так выглядит отказ от newtonsoft json
            // Или не выглядит.. Reference Loop Handling не поддерживается, но обещали подвезти.
            // const string typeName = "Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor,"
            //                     + " Microsoft.AspNetCore.Mvc.Core";
            // Type serviceType = typeof(IActionResultExecutor<JsonResult>);
            // Type implementationType = Type.GetType(typeName);
            //
            // services.AddSingleton(serviceType, implementationType);

            builder.AddNewtonsoftJson();

            return builder;
        }
    }
}