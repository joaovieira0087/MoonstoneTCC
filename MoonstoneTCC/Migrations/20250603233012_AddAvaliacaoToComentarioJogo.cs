using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AddAvaliacaoToComentarioJogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Avaliacao",
                table: "ComentariosJogo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avaliacao",
                table: "ComentariosJogo");
        }
    }
}
