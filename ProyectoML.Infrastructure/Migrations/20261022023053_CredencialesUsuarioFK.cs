using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoML.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CredencialesUsuarioFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "CredencialesMercadoLibre",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CredencialesMercadoLibre_UsuarioId",
                table: "CredencialesMercadoLibre",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_CredencialesMercadoLibre_Usuarios_UsuarioId",
                table: "CredencialesMercadoLibre",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CredencialesMercadoLibre_Usuarios_UsuarioId",
                table: "CredencialesMercadoLibre");

            migrationBuilder.DropIndex(
                name: "IX_CredencialesMercadoLibre_UsuarioId",
                table: "CredencialesMercadoLibre");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "CredencialesMercadoLibre");
        }
    }
}
