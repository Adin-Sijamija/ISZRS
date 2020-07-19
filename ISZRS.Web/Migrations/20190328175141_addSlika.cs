using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ISZRS.Web.Migrations
{
    public partial class addSlika : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Slika",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slika",
                table: "AspNetUsers");
        }
    }
}
