using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using EMWeb.Models;

namespace EMWeb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var appEnv = services.BuildServiceProvider().GetRequiredService<IApplicationEnvironment>();

            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<EMContext>(x => x.UseSqlite("Data source=" + appEnv.ApplicationBasePath + "/Database/EMWeb.db"));

            services.AddIdentity<User, IdentityRole<long>>(x=> {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonLetterOrDigit = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                .AddEntityFrameworkStores<EMContext,long>()
                .AddDefaultTokenProviders();
            services.AddFileUpload()
                .AddEntityFrameworkStorage<EMContext>();
            services.AddMvc();
            services.AddSmartUser<User,long>();

            
        }

        public async void Configure(IApplicationBuilder app,ILoggerFactory loggerFactory)
        {
            app.UseIISPlatformHandler();
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseFileUpload();
            app.UseMvc(x => x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
            await SampleData.InitDB(app.ApplicationServices);
        }

        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
