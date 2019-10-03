using Market.BLL.Interfaces;
using Market.BLL.Services;
using Market.Configuration;
using Market.DAL.Interfaces;
using Market.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace Market.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static void AddTempStorage(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped(typeof(ITempStorage<>), typeof(TempStorage<>));
        }

        public static void AddContentEnvironment<TService>(this IServiceCollection services)
            where TService : class, IContentEnvironment
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddTransient<IContentEnvironment, TService>();
        }

        public static void AddCurrentUserInfo(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<ICurrentUserInfo, CurrentUserInfo>();
        }

        public static void AddCors(this IServiceCollection app, IConfiguration config)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            app.AddCors(options =>
            {
                options.AddPolicy("AllowMyOrigin", builder =>
                {
                    var cors = new Cors();
                    config.GetSection("CORS").Bind(cors);

                    builder.WithOrigins(cors.HttpAddress, cors.HttpAddress)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });

                options.DefaultPolicyName = "AllowMyOrigin";
            });
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "API",
                    Description = "Market API",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }
    }
}
