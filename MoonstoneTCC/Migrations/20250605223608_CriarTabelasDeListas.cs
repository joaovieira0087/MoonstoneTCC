using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelasDeListas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListasJogos",
                columns: table => new
                {
                    ListaJogoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasJogos", x => x.ListaJogoId);
                    table.ForeignKey(
                        name: "FK_ListasJogos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItensListaJogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ListaJogoId = table.Column<int>(type: "int", nullable: false),
                    JogoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensListaJogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensListaJogos_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "JogoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensListaJogos_ListasJogos_ListaJogoId",
                        column: x => x.ListaJogoId,
                        principalTable: "ListasJogos",
                        principalColumn: "ListaJogoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensListaJogos_JogoId",
                table: "ItensListaJogos",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensListaJogos_ListaJogoId",
                table: "ItensListaJogos",
                column: "ListaJogoId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasJogos_UsuarioId",
                table: "ListasJogos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensListaJogos");

            migrationBuilder.DropTable(
                name: "ListasJogos");
        }
    }
}
