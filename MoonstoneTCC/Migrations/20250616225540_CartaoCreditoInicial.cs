using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class CartaoCreditoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartoesCredito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NumeroParcial = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Validade = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    NomeTitular = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoVerificacao = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CartaoPadrao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartoesCredito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartoesCredito_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartoesCredito_UserId",
                table: "CartoesCredito",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartoesCredito");
        }
    }
}
