using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaGamificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GamificacaoUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    XPAtual = table.Column<int>(type: "int", nullable: false),
                    XPTotalAcumulado = table.Column<int>(type: "int", nullable: false),
                    TotalAvaliacoesRealizadas = table.Column<int>(type: "int", nullable: false),
                    ResgatesEfetuados = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamificacaoUsuarios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GamificacaoUsuarios");
        }
    }
}
