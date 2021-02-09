using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GetOrdersFromOpencart.Hubs;
using GetOrdersFromOpencart.Infrastructure;
using GetOrdersFromOpencart.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace GetOrdersFromOpencart
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddHostedService<SendBlazorService>(); //другой вариант - разместить в программ.cs

            services.AddControllersWithViews();
            services.AddScoped<MessageSend>();
            services.AddScoped<Connect>();
            services.AddScoped<SendMessageFromBot>();
            services.AddSingleton<CopyOrdersToNewTable>();
            services.AddTransient<IRepository, WorkRepository>();
            services.AddSingleton<SendBlazorService>();

            services.AddSignalR();
            services.AddServerSideBlazor();
            services.AddRazorPages();
            //services.Configure<RazorPagesOptions>(options => options.RootDirectory = "/Pages");

            services.AddDbContext<ConnectIdentity>(options => options.UseSqlServer(Configuration["ConnectionString:DefaultConnection"]));
            services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;

                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(333);
            })
                .AddEntityFrameworkStores<ConnectIdentity>().AddDefaultTokenProviders();

            services.AddSignalR();
            services.AddCors();


            services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Account/Login");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            var defaultCulture = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            app.UseRequestLocalization(localizationOptions);



            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            //    await next.Invoke();
            //});

            app.UseCors(policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Order", action = "ListOrder" });
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub();

                //endpoints.MapFallbackToPage("/Pages/App");
                // endpoints.MapFallbackToController("Index", "Home");

                endpoints.MapHub<OrderSaveHub>("/notifyfromneworder");
                endpoints.MapFallbackToPage("/_Host");
            });
            // app.UsePathBase("/Home");
        }
    }
}
