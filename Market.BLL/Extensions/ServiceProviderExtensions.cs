using Market.BLL.Interfaces;
using Market.BLL.Services;
using Market.DAL.Entities;
using Market.DAL.Interfaces;
using Market.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Market.BLL.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddEmailSender(this IServiceCollection services)
        {
            services.AddTransient<IEmailSender, EmailSender>();
        }

        public static void AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, EFUnitOfWork>();
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
        }

        public static void AddPasswordGenerator(this IServiceCollection services)
        {
            services.AddTransient<IPasswordGenerator, PasswordGenerator>(x =>
            {
                var passwordOptions = x.GetRequiredService<UserManager<ApplicationUser>>().Options.Password;
                return new PasswordGenerator(passwordOptions);
            });
        }

        public static void AddApplicationDbContext(this IServiceCollection services, string connectionString)
        {
            DAL.Extensions.ServiceProviderExtensions.AddApplicationDbContext(services, connectionString);
        }

        public static void AddApplicationIdentity(this IServiceCollection services)
        {
            DAL.Extensions.ServiceProviderExtensions.AddApplicationIdentity(services);
        }
    }
}