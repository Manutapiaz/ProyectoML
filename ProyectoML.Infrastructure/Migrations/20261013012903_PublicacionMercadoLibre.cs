using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoML.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PublicacionMercadoLibre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicacionesMercadoLibre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Permalink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicacionesMercadoLibre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicacionesMercadoLibre_CredencialesMercadoLibre_UserId",
                        column: x => x.UserId,
                        principalTable: "CredencialesMercadoLibre",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicacionesMercadoLibre_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublicacionesMercadoLibre_ExternalId",
                table: "PublicacionesMercadoLibre",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicacionesMercadoLibre_ProductoId_UserId",
                table: "PublicacionesMercadoLibre",
                columns: new[] { "ProductoId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PublicacionesMercadoLibre_UserId",
                table: "PublicacionesMercadoLibre",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicacionesMercadoLibre");
        }
    }
}
