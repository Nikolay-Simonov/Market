using Market.BLL.Interfaces;
using Market.BLL.Services;
using Market.DAL.Entities;
using Market.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Market.BLL.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEmailSender(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
        }

        /// <summary>
        /// Внедряет менеджеры управления приложением и контентом.
        /// </summary>
        public static void AddApplicationManagers(this IServiceCollection services)
        {
            services.AddScoped<ICategoryManager, CategoryManager>();
            services.AddScoped<ICountryManager, CountryManager>();
            services.AddScoped<ICharacteristicManager, CharacteristicManager>();
            services.AddScoped<IBrandManager, BrandManager>();
            services.AddScoped<IStaffManager, StaffManager>();
            services.AddScoped<IProductManager, ProductManager>();
            services.AddScoped<ICatalogManager, CatalogManager>();
            services.AddScoped<ICartManager>(provider =>
            {
                var userInfo = provider.GetRequiredService<ICurrentUserInfo>();
                var database = provider.GetRequiredService<IUnitOfWork>();

                if (userInfo.IsAuthenticated)
                {
                    return new CartManager(database, userInfo);
                }

                var storage = provider.GetRequiredService<ITempStorage<List<ProductLine>>>();

                return new TempCartManager(storage, database);
            });
        }

        public static void AddPasswordGenerator(this IServiceCollection services)
        {
            services.AddTransient<IPasswordGenerator, PasswordGenerator>(x =>
            {
                var passwordOptions = x.GetRequiredService<UserManager<ApplicationUser>>().Options.Password;
                return new PasswordGenerator(passwordOptions);
            });
        }
    }
}