using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZRS.Data;
using ISZRS.Web.Areas.Identity.Data;
using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Client;
using jsreport.Local;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Utilities;

namespace ISZRS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MojContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("lokalni1")));

            services.AddDbContext<MojContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("fit-server4")));


            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
            });


            services.AddScoped<IViewRenderService, ViewRenderService>();


            //  services.AddJsReport(new ReportingService("http://jsreport:5488"));

            services.AddJsReport(new LocalReporting()
               .UseBinary(JsReportBinary.GetBinary())
               .AsUtility()
               .Create());

            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSession();
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Pocetna}/{action=Index}/{id?}");


                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            CreateRoles(serviceProvider);

        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ZaposlenikISZRSWebUser>>();
            Task<IdentityResult> roleResult;
            string email = "someone@somewhere.com";

            //Check that there is an Administrator role and create if not
            Task<bool> hasAdminRole = roleManager.RoleExistsAsync("Administrator");
            hasAdminRole.Wait();

            if (!hasAdminRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Administrator"));
                roleResult.Wait();
            }

            ProvjeriDodajUlogu("Recepcionar",roleManager);
            ProvjeriDodajUlogu("Logisticar",roleManager);




            //Check if the admin user exists and create it if not
            //Add to the Administrator role

            Task<ZaposlenikISZRSWebUser> testUser = userManager.FindByEmailAsync(email);
            testUser.Wait();

            if (testUser.Result == null)
            {
                ZaposlenikISZRSWebUser Administrator = new ZaposlenikISZRSWebUser();
                Administrator.Email = email;
                Administrator.UserName = "AdinSijamija";
                Administrator.Ime = "Adin";
                Administrator.Prezime = "Sijamija";
                Administrator.JMBG = "1234567890123";


                Task<IdentityResult> newUser = userManager.CreateAsync(Administrator, "AdinSijamija_23");
                newUser.Wait();

                if (newUser.Result.Succeeded)
                {
                    Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(Administrator, "Administrator");
                    newUserRole.Wait();
                }
            }

        }

        private void ProvjeriDodajUlogu( string uloga, RoleManager<IdentityRole> roleManager)
        {
            Task<IdentityResult> roleResult;
            Task<bool> hasAdminRole = roleManager.RoleExistsAsync(uloga);
            hasAdminRole.Wait();

            if (!hasAdminRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole(uloga));
                roleResult.Wait();
            }

        }

    }
}
