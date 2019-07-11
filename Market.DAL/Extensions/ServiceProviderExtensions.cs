using System;
using System.Reflection;
using Market.DAL.EF;
using Market.DAL.Entities;
using Market.DAL.Infrastructure;
using Market.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Market.DAL.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Внедряет контекст базы данных приложения.
        /// </summary>
        /// <exception cref="ArgumentNullException">Параметр <paramref name="services"/>
        /// ссылается на null.</exception>
        /// <exception cref="ArgumentException">Строка <paramref name="connectionString"/>
        /// ссылается на null, пустая или состоит из whitespace символов.</exception>
        public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    $"The argument \"{nameof(connectionString)}\" wass null, empty or whitespace.",
                    nameof(connectionString)
                );
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString,serverOptions =>
                    serverOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name));
            });
        }

        /// <summary>
        /// Внедряет Identity приложения.
        /// </summary>
        /// <exception cref="ArgumentNullException">Параметр <paramref name="services"/>
        /// ссылается на null.</exception>
        public static void AddApplicationIdentity(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddIdentity<ApplicationUser, ApplicationRole>(config =>
                {
                    config.Password.RequireNonAlphanumeric = false;
                    config.Password.RequireUppercase = false;
                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
        }

        /// <summary>
        /// Внедряет строитель SQL команд.
        /// </summary>
        /// <exception cref="ArgumentNullException">Параметр <paramref name="services"/>
        /// ссылается на null.</exception>
        public static void AddFacetsSearch(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton(typeof(IEntityDescription<>), typeof(EFEntityDescription<>));
            services.AddTransient(typeof(IFacetsBuilder<>), typeof(FacetsBuilder<>));
        }
    }
}