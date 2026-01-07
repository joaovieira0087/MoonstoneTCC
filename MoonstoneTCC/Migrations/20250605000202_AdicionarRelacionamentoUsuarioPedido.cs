using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRelacionamentoUsuarioPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Pedidos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_UserId",
                table: "Pedidos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_AspNetUsers_UserId",
                table: "Pedidos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_AspNetUsers_UserId",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_UserId",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Pedidos");
        }
    }
}
