using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoML.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CredencialMercadoLibre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CredencialesMercadoLibre",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredencialesMercadoLibre", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CredencialesMercadoLibre");
        }
    }
}
