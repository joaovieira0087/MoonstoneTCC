using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddPrecoUnitarioToCarrinhoCompraItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoUnitario",
                table: "CarrinhoCompraItens",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoUnitario",
                table: "CarrinhoCompraItens");
        }
    }
}
