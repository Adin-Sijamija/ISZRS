using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ISZRS.Web.Migrations
{
    public partial class Ugovor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UgovoriORadu",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatumPocetkaUgovora = table.Column<DateTime>(nullable: false),
                    DatumZavrsetkaUgovora = table.Column<DateTime>(nullable: false),
                    BrojRadnihSati = table.Column<int>(nullable: false),
                    Satnica = table.Column<float>(nullable: false),
                    PrekovremenaSatnica = table.Column<float>(nullable: false),
                    ZaposlenikKorisnickoIme = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UgovoriORadu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DolasciNaPosao",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrojSati = table.Column<int>(nullable: false),
                    DatumDolaska = table.Column<DateTime>(nullable: false),
                    DatumOdlaska = table.Column<DateTime>(nullable: false),
                    UgovorOraduId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DolasciNaPosao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DolasciNaPosao_UgovoriORadu_UgovorOraduId",
                        column: x => x.UgovorOraduId,
                        principalTable: "UgovoriORadu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Isplata",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Iznos = table.Column<float>(nullable: false),
                    Mjesec = table.Column<int>(nullable: false),
                    Godina = table.Column<int>(nullable: false),
                    UgovoriID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Isplata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Isplata_UgovoriORadu_UgovoriID",
                        column: x => x.UgovoriID,
                        principalTable: "UgovoriORadu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DolasciNaPosao_UgovorOraduId",
                table: "DolasciNaPosao",
                column: "UgovorOraduId");

            migrationBuilder.CreateIndex(
                name: "IX_Isplata_UgovoriID",
                table: "Isplata",
                column: "UgovoriID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DolasciNaPosao");

            migrationBuilder.DropTable(
                name: "Isplata");

            migrationBuilder.DropTable(
                name: "UgovoriORadu");
        }
    }
}
