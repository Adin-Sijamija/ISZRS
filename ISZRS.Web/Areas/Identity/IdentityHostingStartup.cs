using System;
using ISZRS.Web.Areas.Identity.Data;
using ISZRS.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ISZRS.Web.Areas.Identity.IdentityHostingStartup))]
namespace ISZRS.Web.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<ISZRSWebContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("lokalni1")));

                //services.AddDbContext<ISZRSWebContext>(options =>
                //options.UseSqlServer(
                //    context.Configuration.GetConnectionString("fit-server4")));



                services.AddIdentity<ZaposlenikISZRSWebUser, IdentityRole>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddDefaultUI()
                    .AddDefaultTokenProviders()
                    .AddEntityFrameworkStores<ISZRSWebContext>();
            });
        }
    }
}