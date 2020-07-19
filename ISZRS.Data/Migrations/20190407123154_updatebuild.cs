using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ISZRS.Data.Migrations
{
    public partial class updatebuild : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drzave",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drzave", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hrana",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(maxLength: 30, nullable: false),
                    Opis = table.Column<string>(maxLength: 30, nullable: false),
                    Cijena = table.Column<float>(nullable: false),
                    Slika = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hrana", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Namjestaj",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ime = table.Column<string>(maxLength: 30, nullable: false),
                    Opis = table.Column<string>(maxLength: 100, nullable: false),
                    Cijena = table.Column<float>(nullable: false),
                    tipNamjestaja = table.Column<int>(nullable: false),
                    JeOsnovniNamjestaj = table.Column<bool>(nullable: false),
                    Slika = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Namjestaj", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipKartice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipKartice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipSobe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(maxLength: 30, nullable: false),
                    Opis = table.Column<string>(maxLength: 100, nullable: false),
                    Kapacitet = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipSobe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usluge",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(maxLength: 30, nullable: false),
                    Opis = table.Column<string>(maxLength: 30, nullable: false),
                    Cijena = table.Column<float>(nullable: false),
                    TipCijene = table.Column<int>(nullable: false),
                    TipUsluge = table.Column<int>(nullable: false),
                    Slika = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usluge", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gradovi",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(maxLength: 50, nullable: false),
                    Regija = table.Column<string>(maxLength: 100, nullable: false),
                    DrzavaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gradovi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gradovi_Drzave_DrzavaId",
                        column: x => x.DrzavaId,
                        principalTable: "Drzave",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KreditneKartice",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VaziDo = table.Column<string>(nullable: false),
                    VaziDoDatum = table.Column<DateTime>(nullable: false),
                    BrojKartice = table.Column<string>(maxLength: 20, nullable: false),
                    CVV = table.Column<string>(maxLength: 5, nullable: false),
                    TipKarticeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KreditneKartice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KreditneKartice_TipKartice_TipKarticeID",
                        column: x => x.TipKarticeID,
                        principalTable: "TipKartice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sobe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrojSobe = table.Column<int>(nullable: false),
                    BrojSprata = table.Column<int>(nullable: false),
                    Cijena = table.Column<float>(nullable: false),
                    TipSobeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sobe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sobe_TipSobe_TipSobeID",
                        column: x => x.TipSobeID,
                        principalTable: "TipSobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipSobeMoguciNamjestaj",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NamjestajID = table.Column<int>(nullable: false),
                    TipSobeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipSobeMoguciNamjestaj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipSobeMoguciNamjestaj_Namjestaj_NamjestajID",
                        column: x => x.NamjestajID,
                        principalTable: "Namjestaj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TipSobeMoguciNamjestaj_TipSobe_TipSobeID",
                        column: x => x.TipSobeID,
                        principalTable: "TipSobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipSobeNamjestaj",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TipSobeID = table.Column<int>(nullable: false),
                    TipNamjestaja = table.Column<int>(nullable: false),
                    Kolicina = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipSobeNamjestaj", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TipSobeNamjestaj_TipSobe_TipSobeID",
                        column: x => x.TipSobeID,
                        principalTable: "TipSobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dodaci",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ime = table.Column<string>(maxLength: 30, nullable: false),
                    Opis = table.Column<string>(maxLength: 30, nullable: false),
                    Cijena = table.Column<float>(nullable: false),
                    JeUkljucen = table.Column<bool>(nullable: false),
                    Slika = table.Column<byte[]>(nullable: false),
                    UslugeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dodaci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dodaci_Usluge_UslugeID",
                        column: x => x.UslugeID,
                        principalTable: "Usluge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Osobe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Ime = table.Column<string>(maxLength: 50, nullable: false),
                    Prezime = table.Column<string>(maxLength: 50, nullable: false),
                    Adresa = table.Column<string>(maxLength: 50, nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    KreditneKarticaId = table.Column<int>(nullable: true),
                    GradId = table.Column<int>(nullable: true),
                    JMBG = table.Column<string>(maxLength: 13, nullable: true),
                    KorisnickoIme = table.Column<string>(maxLength: 50, nullable: true),
                    Sifra = table.Column<string>(maxLength: 50, nullable: true),
                    BrojTelefona = table.Column<string>(maxLength: 50, nullable: true),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    MjestoBoravka = table.Column<string>(maxLength: 50, nullable: true),
                    DatumRođenja = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Osobe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Osobe_Gradovi_GradId",
                        column: x => x.GradId,
                        principalTable: "Gradovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Osobe_KreditneKartice_KreditneKarticaId",
                        column: x => x.KreditneKarticaId,
                        principalTable: "KreditneKartice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rezervacije",
                columns: table => new
                {
                    RezervacijaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatumPocetkaRezerviranja = table.Column<DateTime>(nullable: false),
                    DatumZavrsetkaRezerviranja = table.Column<DateTime>(nullable: false),
                    DatumEvidentiranjaRezerviranja = table.Column<DateTime>(nullable: false),
                    RezervacijaAktivna = table.Column<bool>(nullable: false),
                    RezervacijaZavrsena = table.Column<bool>(nullable: false),
                    GlavniGostId = table.Column<int>(nullable: true),
                    Zaposlenik = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervacije", x => x.RezervacijaId);
                    table.ForeignKey(
                        name: "FK_Rezervacije_Osobe_GlavniGostId",
                        column: x => x.GlavniGostId,
                        principalTable: "Osobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Racun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PdfDoc = table.Column<byte[]>(nullable: true),
                    datumKreacije = table.Column<DateTime>(nullable: false),
                    RezervacijaId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racun", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Racun_Rezervacije_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacije",
                        principalColumn: "RezervacijaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZaduzeneSobe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SobaID = table.Column<int>(nullable: false),
                    RezervacijaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaduzeneSobe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZaduzeneSobe_Rezervacije_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacije",
                        principalColumn: "RezervacijaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ZaduzeneSobe_Sobe_SobaID",
                        column: x => x.SobaID,
                        principalTable: "Sobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zaduzivanja",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RezervacijaId = table.Column<int>(nullable: false),
                    UslugaID = table.Column<int>(nullable: false),
                    PocetakZaduzivanja = table.Column<DateTime>(nullable: false),
                    KrajZaduzivanja = table.Column<DateTime>(nullable: false),
                    JeZavršeno = table.Column<bool>(nullable: false),
                    UkupnaCijena = table.Column<float>(nullable: false),
                    ZaposleniciId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zaduzivanja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zaduzivanja_Rezervacije_RezervacijaId",
                        column: x => x.RezervacijaId,
                        principalTable: "Rezervacije",
                        principalColumn: "RezervacijaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zaduzivanja_Usluge_UslugaID",
                        column: x => x.UslugaID,
                        principalTable: "Usluge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zaduzivanja_Osobe_ZaposleniciId",
                        column: x => x.ZaposleniciId,
                        principalTable: "Osobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GostaSoba",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ZaduzenaSobaID = table.Column<int>(nullable: false),
                    GostiID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GostaSoba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GostaSoba_Osobe_GostiID",
                        column: x => x.GostiID,
                        principalTable: "Osobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GostaSoba_ZaduzeneSobe_ZaduzenaSobaID",
                        column: x => x.ZaduzenaSobaID,
                        principalTable: "ZaduzeneSobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NamjestajSoba",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SobeID = table.Column<int>(nullable: false),
                    NamjestajID = table.Column<int>(nullable: false),
                    ZaduzenaSobaID = table.Column<int>(nullable: true),
                    Kolicina = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NamjestajSoba", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NamjestajSoba_Namjestaj_NamjestajID",
                        column: x => x.NamjestajID,
                        principalTable: "Namjestaj",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NamjestajSoba_Sobe_SobeID",
                        column: x => x.SobeID,
                        principalTable: "Sobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NamjestajSoba_ZaduzeneSobe_ZaduzenaSobaID",
                        column: x => x.ZaduzenaSobaID,
                        principalTable: "ZaduzeneSobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Narudzbe",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatumNarudzbe = table.Column<DateTime>(nullable: false),
                    DatumDostave = table.Column<DateTime>(nullable: false),
                    UkupnaCijena = table.Column<float>(nullable: false),
                    ZaduzenaSobaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narudzbe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Narudzbe_ZaduzeneSobe_ZaduzenaSobaId",
                        column: x => x.ZaduzenaSobaId,
                        principalTable: "ZaduzeneSobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GostiUsluga",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ZaduzivanjaID = table.Column<int>(nullable: false),
                    GostiID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GostiUsluga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GostiUsluga_Osobe_GostiID",
                        column: x => x.GostiID,
                        principalTable: "Osobe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GostiUsluga_Zaduzivanja_ZaduzivanjaID",
                        column: x => x.ZaduzivanjaID,
                        principalTable: "Zaduzivanja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UslugaDodaciZaduzenje",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UslugaID = table.Column<int>(nullable: true),
                    DodaciID = table.Column<int>(nullable: true),
                    ZaduzivanjaID = table.Column<int>(nullable: false),
                    Kolicina = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UslugaDodaciZaduzenje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UslugaDodaciZaduzenje_Dodaci_DodaciID",
                        column: x => x.DodaciID,
                        principalTable: "Dodaci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UslugaDodaciZaduzenje_Usluge_UslugaID",
                        column: x => x.UslugaID,
                        principalTable: "Usluge",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UslugaDodaciZaduzenje_Zaduzivanja_ZaduzivanjaID",
                        column: x => x.ZaduzivanjaID,
                        principalTable: "Zaduzivanja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NarudzbaHrana",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NarudzbeID = table.Column<int>(nullable: false),
                    HranaID = table.Column<int>(nullable: false),
                    Kolicina = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NarudzbaHrana", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NarudzbaHrana_Hrana_HranaID",
                        column: x => x.HranaID,
                        principalTable: "Hrana",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NarudzbaHrana_Narudzbe_NarudzbeID",
                        column: x => x.NarudzbeID,
                        principalTable: "Narudzbe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dodaci_UslugeID",
                table: "Dodaci",
                column: "UslugeID");

            migrationBuilder.CreateIndex(
                name: "IX_GostaSoba_GostiID",
                table: "GostaSoba",
                column: "GostiID");

            migrationBuilder.CreateIndex(
                name: "IX_GostaSoba_ZaduzenaSobaID",
                table: "GostaSoba",
                column: "ZaduzenaSobaID");

            migrationBuilder.CreateIndex(
                name: "IX_GostiUsluga_GostiID",
                table: "GostiUsluga",
                column: "GostiID");

            migrationBuilder.CreateIndex(
                name: "IX_GostiUsluga_ZaduzivanjaID",
                table: "GostiUsluga",
                column: "ZaduzivanjaID");

            migrationBuilder.CreateIndex(
                name: "IX_Gradovi_DrzavaId",
                table: "Gradovi",
                column: "DrzavaId");

            migrationBuilder.CreateIndex(
                name: "IX_KreditneKartice_TipKarticeID",
                table: "KreditneKartice",
                column: "TipKarticeID");

            migrationBuilder.CreateIndex(
                name: "IX_NamjestajSoba_NamjestajID",
                table: "NamjestajSoba",
                column: "NamjestajID");

            migrationBuilder.CreateIndex(
                name: "IX_NamjestajSoba_SobeID",
                table: "NamjestajSoba",
                column: "SobeID");

            migrationBuilder.CreateIndex(
                name: "IX_NamjestajSoba_ZaduzenaSobaID",
                table: "NamjestajSoba",
                column: "ZaduzenaSobaID");

            migrationBuilder.CreateIndex(
                name: "IX_NarudzbaHrana_HranaID",
                table: "NarudzbaHrana",
                column: "HranaID");

            migrationBuilder.CreateIndex(
                name: "IX_NarudzbaHrana_NarudzbeID",
                table: "NarudzbaHrana",
                column: "NarudzbeID");

            migrationBuilder.CreateIndex(
                name: "IX_Narudzbe_ZaduzenaSobaId",
                table: "Narudzbe",
                column: "ZaduzenaSobaId");

            migrationBuilder.CreateIndex(
                name: "IX_Osobe_GradId",
                table: "Osobe",
                column: "GradId");

            migrationBuilder.CreateIndex(
                name: "IX_Osobe_KreditneKarticaId",
                table: "Osobe",
                column: "KreditneKarticaId");

            migrationBuilder.CreateIndex(
                name: "IX_Racun_RezervacijaId",
                table: "Racun",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Rezervacije_GlavniGostId",
                table: "Rezervacije",
                column: "GlavniGostId");

            migrationBuilder.CreateIndex(
                name: "IX_Sobe_TipSobeID",
                table: "Sobe",
                column: "TipSobeID");

            migrationBuilder.CreateIndex(
                name: "IX_TipSobeMoguciNamjestaj_NamjestajID",
                table: "TipSobeMoguciNamjestaj",
                column: "NamjestajID");

            migrationBuilder.CreateIndex(
                name: "IX_TipSobeMoguciNamjestaj_TipSobeID",
                table: "TipSobeMoguciNamjestaj",
                column: "TipSobeID");

            migrationBuilder.CreateIndex(
                name: "IX_TipSobeNamjestaj_TipSobeID",
                table: "TipSobeNamjestaj",
                column: "TipSobeID");

            migrationBuilder.CreateIndex(
                name: "IX_UslugaDodaciZaduzenje_DodaciID",
                table: "UslugaDodaciZaduzenje",
                column: "DodaciID");

            migrationBuilder.CreateIndex(
                name: "IX_UslugaDodaciZaduzenje_UslugaID",
                table: "UslugaDodaciZaduzenje",
                column: "UslugaID");

            migrationBuilder.CreateIndex(
                name: "IX_UslugaDodaciZaduzenje_ZaduzivanjaID",
                table: "UslugaDodaciZaduzenje",
                column: "ZaduzivanjaID");

            migrationBuilder.CreateIndex(
                name: "IX_ZaduzeneSobe_RezervacijaId",
                table: "ZaduzeneSobe",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_ZaduzeneSobe_SobaID",
                table: "ZaduzeneSobe",
                column: "SobaID");

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzivanja_RezervacijaId",
                table: "Zaduzivanja",
                column: "RezervacijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzivanja_UslugaID",
                table: "Zaduzivanja",
                column: "UslugaID");

            migrationBuilder.CreateIndex(
                name: "IX_Zaduzivanja_ZaposleniciId",
                table: "Zaduzivanja",
                column: "ZaposleniciId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GostaSoba");

            migrationBuilder.DropTable(
                name: "GostiUsluga");

            migrationBuilder.DropTable(
                name: "NamjestajSoba");

            migrationBuilder.DropTable(
                name: "NarudzbaHrana");

            migrationBuilder.DropTable(
                name: "Racun");

            migrationBuilder.DropTable(
                name: "TipSobeMoguciNamjestaj");

            migrationBuilder.DropTable(
                name: "TipSobeNamjestaj");

            migrationBuilder.DropTable(
                name: "UslugaDodaciZaduzenje");

            migrationBuilder.DropTable(
                name: "Hrana");

            migrationBuilder.DropTable(
                name: "Narudzbe");

            migrationBuilder.DropTable(
                name: "Namjestaj");

            migrationBuilder.DropTable(
                name: "Dodaci");

            migrationBuilder.DropTable(
                name: "Zaduzivanja");

            migrationBuilder.DropTable(
                name: "ZaduzeneSobe");

            migrationBuilder.DropTable(
                name: "Usluge");

            migrationBuilder.DropTable(
                name: "Rezervacije");

            migrationBuilder.DropTable(
                name: "Sobe");

            migrationBuilder.DropTable(
                name: "Osobe");

            migrationBuilder.DropTable(
                name: "TipSobe");

            migrationBuilder.DropTable(
                name: "Gradovi");

            migrationBuilder.DropTable(
                name: "KreditneKartice");

            migrationBuilder.DropTable(
                name: "Drzave");

            migrationBuilder.DropTable(
                name: "TipKartice");
        }
    }
}
