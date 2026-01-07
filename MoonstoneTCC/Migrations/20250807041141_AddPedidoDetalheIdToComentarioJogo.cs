using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddPedidoDetalheIdToComentarioJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PedidoDetalheId",
                table: "ComentariosJogo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosJogo_PedidoDetalheId",
                table: "ComentariosJogo",
                column: "PedidoDetalheId");

            migrationBuilder.AddForeignKey(
                name: "FK_ComentariosJogo_PedidoDetalhes_PedidoDetalheId",
                table: "ComentariosJogo",
                column: "PedidoDetalheId",
                principalTable: "PedidoDetalhes",
                principalColumn: "PedidoDetalheId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ComentariosJogo_PedidoDetalhes_PedidoDetalheId",
                table: "ComentariosJogo");

            migrationBuilder.DropIndex(
                name: "IX_ComentariosJogo_PedidoDetalheId",
                table: "ComentariosJogo");

            migrationBuilder.DropColumn(
                name: "PedidoDetalheId",
                table: "ComentariosJogo");
        }
    }
}
