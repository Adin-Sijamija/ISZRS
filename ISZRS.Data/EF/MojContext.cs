using ISZR.Web.Models;
using ISZRS.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;


namespace ISZRS.Data
{
    public class MojContext : DbContext
    {
        public MojContext(DbContextOptions<MojContext> options)
           : base(options)
        {

        }


        public DbSet<Dodaci> Dodaci { get; set; }
        public DbSet<Drzave> Drzave { get; set; }
        public DbSet<GostaSoba> GostaSoba { get; set; }
        public DbSet<Gosti> Gosti { get; set; }
        public DbSet<Gradovi> Gradovi { get; set; }
        public DbSet<Hrana> Hrana { get; set; }
        public DbSet<KreditneKartice> KreditneKartice { get; set; }
        public DbSet<Namjestaj> Namjestaj { get; set; }
        public DbSet<NamjestajSoba> NamjestajSoba { get; set; }
        public DbSet<NarudzbaHrana> NarudzbaHrana { get; set; }
        public DbSet<Narudzbe> Narudzbe { get; set; }
        public DbSet<Sobe> Sobe { get; set; }
        public DbSet<TipKartice> TipKartice { get; set; }
        public DbSet<TipSobe> TipSobe { get; set; }
        public DbSet<Usluge> Usluge { get; set; }
        public DbSet<ZaduzeneSobe> ZaduzeneSobe { get; set; }
        public DbSet<Zaduzivanja> Zaduzivanja { get; set; }
        public DbSet<Zaposlenici> Zaposlenici { get; set; }
        public DbSet<Osobe> Osobe { get; set; }
        public DbSet<Rezervacije> Rezervacije { get; set; }
        public DbSet<TipSobeNamjestaj> TipSobeNamjestaj { get; set; }     
        public DbSet<GostiUsluga> GostiUsluga { get; set; }
        public DbSet<UslugaDodaciZaduzenje> UslugaDodaciZaduzenje { get; set; }
        public DbSet<TipSobeMoguciNamjestaj> TipSobeMoguciNamjestaj { get; set; }
        public DbSet<Racun> Racun { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=.\;Database=Recepcijav2;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          

        }



    }

}

