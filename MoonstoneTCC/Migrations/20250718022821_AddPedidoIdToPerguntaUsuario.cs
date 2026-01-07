using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddPedidoIdToPerguntaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PedidoId",
                table: "PerguntasUsuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerguntasUsuarios_PedidoId",
                table: "PerguntasUsuarios",
                column: "PedidoId");

            migrationBuilder.AddForeignKey(
                name: "FK_PerguntasUsuarios_Pedidos_PedidoId",
                table: "PerguntasUsuarios",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "PedidoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerguntasUsuarios_Pedidos_PedidoId",
                table: "PerguntasUsuarios");

            migrationBuilder.DropIndex(
                name: "IX_PerguntasUsuarios_PedidoId",
                table: "PerguntasUsuarios");

            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "PerguntasUsuarios");
        }
    }
}
