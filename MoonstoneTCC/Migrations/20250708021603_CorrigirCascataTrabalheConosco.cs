using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirCascataTrabalheConosco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VagasEmprego",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Requisitos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salario = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VagasEmprego", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    VagaEmpregoId = table.Column<int>(type: "int", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CPF = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RG = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidaturas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Candidaturas_VagasEmprego_VagaEmpregoId",
                        column: x => x.VagaEmpregoId,
                        principalTable: "VagasEmprego",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerguntasVaga",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VagaEmpregoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerguntasVaga", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerguntasVaga_VagasEmprego_VagaEmpregoId",
                        column: x => x.VagaEmpregoId,
                        principalTable: "VagasEmprego",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RespostasCandidatura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidaturaId = table.Column<int>(type: "int", nullable: false),
                    PerguntaVagaId = table.Column<int>(type: "int", nullable: false),
                    RespostaTexto = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespostasCandidatura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RespostasCandidatura_Candidaturas_CandidaturaId",
                        column: x => x.CandidaturaId,
                        principalTable: "Candidaturas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespostasCandidatura_PerguntasVaga_PerguntaVagaId",
                        column: x => x.PerguntaVagaId,
                        principalTable: "PerguntasVaga",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_UsuarioId",
                table: "Candidaturas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_VagaEmpregoId",
                table: "Candidaturas",
                column: "VagaEmpregoId");

            migrationBuilder.CreateIndex(
                name: "IX_PerguntasVaga_VagaEmpregoId",
                table: "PerguntasVaga",
                column: "VagaEmpregoId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasCandidatura_CandidaturaId",
                table: "RespostasCandidatura",
                column: "CandidaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespostasCandidatura_PerguntaVagaId",
                table: "RespostasCandidatura",
                column: "PerguntaVagaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RespostasCandidatura");

            migrationBuilder.DropTable(
                name: "Candidaturas");

            migrationBuilder.DropTable(
                name: "PerguntasVaga");

            migrationBuilder.DropTable(
                name: "VagasEmprego");
        }
    }
}
