using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ISZR.Web.Models;
using ISZRS.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ISZRS.Web.Models
{
    public class ISZRSWebContext : IdentityDbContext<ZaposlenikISZRSWebUser,IdentityRole,string>
    {
        public ISZRSWebContext(DbContextOptions<ISZRSWebContext> options)
            : base(options)
        {
        }

        public DbSet<Ugovori> UgovoriORadu { get; set; }
        public DbSet<DolasciNaPosao> DolasciNaPosao { get; set; }
        public DbSet<Isplata> Isplata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<IdentityUser>(b =>
            //{
            //    b.ToTable("Zaposlenik");
            //});

            //modelBuilder.Entity<IdentityRole>(b =>
            //{
            //    b.ToTable("Odjel");
            //});




        }
    }
}
