using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaSeguidores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeguidoresUsuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeguidorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeguidoId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguidoresUsuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeguidoresUsuarios_AspNetUsers_SeguidoId",
                        column: x => x.SeguidoId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeguidoresUsuarios_AspNetUsers_SeguidorId",
                        column: x => x.SeguidorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeguidoresUsuarios_SeguidoId",
                table: "SeguidoresUsuarios",
                column: "SeguidoId");

            migrationBuilder.CreateIndex(
                name: "IX_SeguidoresUsuarios_SeguidorId",
                table: "SeguidoresUsuarios",
                column: "SeguidorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeguidoresUsuarios");
        }
    }
}
