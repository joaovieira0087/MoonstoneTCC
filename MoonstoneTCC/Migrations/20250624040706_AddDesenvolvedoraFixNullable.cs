using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddDesenvolvedoraFixNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DesenvolvedoraId",
                table: "Jogos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Desenvolvedoras",
                columns: table => new
                {
                    DesenvolvedoraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    FotoPerfilUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Curiosidades = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SlideImagens = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desenvolvedoras", x => x.DesenvolvedoraId);
                });

            migrationBuilder.CreateTable(
                name: "SeguidoresDesenvolvedoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DesenvolvedoraId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguidoresDesenvolvedoras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeguidoresDesenvolvedoras_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SeguidoresDesenvolvedoras_Desenvolvedoras_DesenvolvedoraId",
                        column: x => x.DesenvolvedoraId,
                        principalTable: "Desenvolvedoras",
                        principalColumn: "DesenvolvedoraId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_DesenvolvedoraId",
                table: "Jogos",
                column: "DesenvolvedoraId");

            migrationBuilder.CreateIndex(
                name: "IX_SeguidoresDesenvolvedoras_DesenvolvedoraId",
                table: "SeguidoresDesenvolvedoras",
                column: "DesenvolvedoraId");

            migrationBuilder.CreateIndex(
                name: "IX_SeguidoresDesenvolvedoras_UsuarioId",
                table: "SeguidoresDesenvolvedoras",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jogos_Desenvolvedoras_DesenvolvedoraId",
                table: "Jogos",
                column: "DesenvolvedoraId",
                principalTable: "Desenvolvedoras",
                principalColumn: "DesenvolvedoraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jogos_Desenvolvedoras_DesenvolvedoraId",
                table: "Jogos");

            migrationBuilder.DropTable(
                name: "SeguidoresDesenvolvedoras");

            migrationBuilder.DropTable(
                name: "Desenvolvedoras");

            migrationBuilder.DropIndex(
                name: "IX_Jogos_DesenvolvedoraId",
                table: "Jogos");

            migrationBuilder.DropColumn(
                name: "DesenvolvedoraId",
                table: "Jogos");
        }
    }
}
