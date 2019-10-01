using Market.BLL.Interfaces;
using Market.BLL.Services;
using Market.DAL.Interfaces;
using Market.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Market.Extensions
{
    public static class ServiceCollectionExtensions
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
    }
}
