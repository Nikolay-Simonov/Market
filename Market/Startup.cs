using Market.BLL.Extensions;
using Market.DAL.Extensions;
using Market.DAL.Interfaces;
using Market.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Market
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region DAL Services

            services.AddTransient<IContentEnvironment, ContentEnvironment>();
            services.AddApplicationDbContext(Configuration.GetConnectionString("DefaultConnection"));
            services.AddApplicationIdentity();
            services.AddFacetsSearch();

            #endregion

            #region BLL Services

            services.AddEmailSender();
            services.AddPasswordGenerator();
            services.AddUnitOfWork();
            services.AddApplicationManagers();

            #endregion

            services.AddControllersWithViews()
                .AddNewtonsoftJson();
            // Тосты практически не работают (работают через раз) в третьей версии из-за бага
            // Http.DefaultHttpContext.get_Items() третьей версии кора
            // .AddRazorRuntimeCompilation(); // в Core 3.0 необходима для работы NToastNotify
            // .AddNToastNotifyNoty(new NotyOptions
            // {
            //     Timeout = 5000,
            //     ProgressBar = true
            // });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.EnsureAdmin();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            //app.UseNToastNotify();
            app.ChangeCurrentLocale();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    new { Controller = "Home", Action = "Index"});
            });
        }
    }
}