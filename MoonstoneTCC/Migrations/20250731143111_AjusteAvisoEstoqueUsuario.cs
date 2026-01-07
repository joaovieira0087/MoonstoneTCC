using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AjusteAvisoEstoqueUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvisosEstoque_Jogos_JogoId",
                table: "AvisosEstoque");

            migrationBuilder.AlterColumn<int>(
                name: "JogoId",
                table: "AvisosEstoque",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AcessorioId",
                table: "AvisosEstoque",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvisosEstoque_AcessorioId",
                table: "AvisosEstoque",
                column: "AcessorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvisosEstoque_Acessorios_AcessorioId",
                table: "AvisosEstoque",
                column: "AcessorioId",
                principalTable: "Acessorios",
                principalColumn: "AcessorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvisosEstoque_Jogos_JogoId",
                table: "AvisosEstoque",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvisosEstoque_Acessorios_AcessorioId",
                table: "AvisosEstoque");

            migrationBuilder.DropForeignKey(
                name: "FK_AvisosEstoque_Jogos_JogoId",
                table: "AvisosEstoque");

            migrationBuilder.DropIndex(
                name: "IX_AvisosEstoque_AcessorioId",
                table: "AvisosEstoque");

            migrationBuilder.DropColumn(
                name: "AcessorioId",
                table: "AvisosEstoque");

            migrationBuilder.AlterColumn<int>(
                name: "JogoId",
                table: "AvisosEstoque",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AvisosEstoque_Jogos_JogoId",
                table: "AvisosEstoque",
                column: "JogoId",
                principalTable: "Jogos",
                principalColumn: "JogoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
