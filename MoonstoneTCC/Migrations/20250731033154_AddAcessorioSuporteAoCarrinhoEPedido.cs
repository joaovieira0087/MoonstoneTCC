using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddAcessorioSuporteAoCarrinhoEPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalhes_Jogos_JogoId",
                table: "PedidoDetalhes");

            migrationBuilder.AlterColumn<int>(
                name: "JogoId",
                table: "PedidoDetalhes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AcessorioId",
                table: "PedidoDetalhes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcessorioId",
                table: "CarrinhoCompraItens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Acessorios",
                columns: table => new
                {
                    AcessorioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DescricaoCurta = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DescricaoDetalhada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrecoPromocional = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ImagemUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagemThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagensAdicionais = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagensAdicionais2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagensAdicionais3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Marca = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Conectividade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Compatibilidade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Material = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Peso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dimensoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bateria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Garantia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItensInclusos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InformacoesTecnicas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InformacoesExtras = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassificacaoUso = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantidadeEstoque = table.Column<int>(type: "int", nullable: false),
                    EstoqueMinimoAlerta = table.Column<int>(type: "int", nullable: false),
                    IsAcessorioDestaque = table.Column<bool>(type: "bit", nullable: false),
                    ProdutoNovo = table.Column<bool>(type: "bit", nullable: false),
                    ProdutoMaisVendido = table.Column<bool>(type: "bit", nullable: false),
                    AvaliacaoMedia = table.Column<double>(type: "float", nullable: true),
                    TotalAvaliacoes = table.Column<int>(type: "int", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Acessorios", x => x.AcessorioId);
                    table.ForeignKey(
                        name: "FK_Acessorios_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "CategoriaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalhes_AcessorioId",
                table: "PedidoDetalhes",
                column: "AcessorioId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoCompraItens_AcessorioId",
                table: "CarrinhoCompraItens",
                column: "AcessorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Acessorios_CategoriaId",
                table: "Acessorios",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrinhoCompraItens_Acessorios_AcessorioId",
                table: "CarrinhoCompraItens",
                column: "AcessorioId",
                principalTable: "Acessorios",
                principalColumn: "AcessorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalhes_Acessorios_AcessorioId",
                table: "PedidoDetalhes",
                column: "AcessorioId",
                principalTable: "Acessorios",
                principalColumn: "AcessorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalhes_Jogos_JogoId",
                table: "PedidoDetalhes",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrinhoCompraItens_Acessorios_AcessorioId",
                table: "CarrinhoCompraItens");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalhes_Acessorios_AcessorioId",
                table: "PedidoDetalhes");

            migrationBuilder.DropForeignKey(
                name: "FK_PedidoDetalhes_Jogos_JogoId",
                table: "PedidoDetalhes");

            migrationBuilder.DropTable(
                name: "Acessorios");

            migrationBuilder.DropIndex(
                name: "IX_PedidoDetalhes_AcessorioId",
                table: "PedidoDetalhes");

            migrationBuilder.DropIndex(
                name: "IX_CarrinhoCompraItens_AcessorioId",
                table: "CarrinhoCompraItens");

            migrationBuilder.DropColumn(
                name: "AcessorioId",
                table: "PedidoDetalhes");

            migrationBuilder.DropColumn(
                name: "AcessorioId",
                table: "CarrinhoCompraItens");

            migrationBuilder.AlterColumn<int>(
                name: "JogoId",
                table: "PedidoDetalhes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PedidoDetalhes_Jogos_JogoId",
                table: "PedidoDetalhes",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
