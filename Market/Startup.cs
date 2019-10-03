using Market.BLL.Extensions;
using Market.DAL.Extensions;
using Market.Extensions;
using Market.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;

namespace Market
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            };
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options => options.IdleTimeout = TimeSpan.FromHours(24));
            services.AddCors(Configuration);

            #region DAL Services

            services.AddTempStorage();
            services.AddContentEnvironment<ContentEnvironment>();
            services.AddApplicationDbContext(Configuration.GetConnectionString("DefaultConnection"));
            services.AddApplicationIdentity();
            services.AddUnitOfWork();
            services.AddFacetsSearch();

            #endregion

            #region BLL Services

            services.AddCurrentUserInfo();
            services.AddEmailSender();
            services.AddPasswordGenerator();
            services.AddApplicationManagers();

            #endregion

            services.AddControllersWithViews().AddJson();
            services.AddSwagger();
            // Тосты практически не работают (работают через раз) в третьей версии из-за бага
            // Http.DefaultHttpContext.get_Items()
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
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCookiePolicy();
            app.UseSession();
            //app.UseNToastNotify();
            app.ChangeCurrentLocale();
            app.UseCors();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    new { Controller = "Home", Action = "Index"});
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Market API V1");
            });
        }
    }
}