using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AjudaPedidoCriacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AjudasPedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TipoProblema = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespostaAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataResposta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resolvido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AjudasPedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AjudasPedidos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AjudasPedidos_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "PedidoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AjudasPedidos_PedidoId",
                table: "AjudasPedidos",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_AjudasPedidos_UsuarioId",
                table: "AjudasPedidos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AjudasPedidos");
        }
    }
}
