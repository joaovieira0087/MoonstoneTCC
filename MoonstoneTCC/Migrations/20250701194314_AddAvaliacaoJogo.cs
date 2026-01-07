using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddAvaliacaoJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvaliacoesJogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    JogoId = table.Column<int>(type: "int", nullable: false),
                    Gostou = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesJogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesJogos_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvaliacoesJogos_Jogos_JogoId",
                        column: x => x.JogoId,
                        principalTable: "Jogos",
                        principalColumn: "JogoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesJogos_JogoId",
                table: "AvaliacoesJogos",
                column: "JogoId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesJogos_UsuarioId",
                table: "AvaliacoesJogos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvaliacoesJogos");
        }
    }
}
