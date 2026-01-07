using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaCamposEstoqueJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CancelamentosPedidos_PedidoId",
                table: "CancelamentosPedidos");

            migrationBuilder.AddColumn<int>(
                name: "EstoqueMinimoAlerta",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadeEstoque",
                table: "Jogos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AvisosEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JogoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Avisado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvisosEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvisosEstoque_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AvisosEstoque_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "JogoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JogoId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacoesEstoque_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificacoesEstoque_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "JogoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CancelamentosPedidos_PedidoId",
                table: "CancelamentosPedidos",
                column: "PedidoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvisosEstoque_JogoId",
                table: "AvisosEstoque",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvisosEstoque_UsuarioId",
                table: "AvisosEstoque",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesEstoque_JogoId",
                table: "NotificacoesEstoque",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacoesEstoque_UsuarioId",
                table: "NotificacoesEstoque",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvisosEstoque");

            migrationBuilder.DropTable(
                name: "NotificacoesEstoque");

            migrationBuilder.DropIndex(
                name: "IX_CancelamentosPedidos_PedidoId",
                table: "CancelamentosPedidos");

            migrationBuilder.DropColumn(
                name: "EstoqueMinimoAlerta",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "QuantidadeEstoque",
                table: "Jogos");

            migrationBuilder.CreateIndex(
                name: "IX_CancelamentosPedidos_PedidoId",
                table: "CancelamentosPedidos",
                column: "PedidoId");
        }
    }
}
