using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoML.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IndiceUnicoVentaMovimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovimientoStock_ProductoId",
                table: "MovimientoStock");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoStock_ProductoId_Referencia",
                table: "MovimientoStock",
                columns: new[] { "ProductoId", "Referencia" },
                unique: true,
                filter: "[Motivo] = 'Venta'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MovimientoStock_ProductoId_Referencia",
                table: "MovimientoStock");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoStock_ProductoId",
                table: "MovimientoStock",
                column: "ProductoId");
        }
    }
}
