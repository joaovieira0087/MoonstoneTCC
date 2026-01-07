using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaRelacionamentoEntrevista : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntrevistaAgendada_Candidaturas_CandidaturaId",
                table: "EntrevistaAgendada");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntrevistaAgendada",
                table: "EntrevistaAgendada");

            migrationBuilder.RenameTable(
                name: "EntrevistaAgendada",
                newName: "EntrevistasAgendadas");

            migrationBuilder.RenameIndex(
                name: "IX_EntrevistaAgendada_CandidaturaId",
                table: "EntrevistasAgendadas",
                newName: "IX_EntrevistasAgendadas_CandidaturaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntrevistasAgendadas",
                table: "EntrevistasAgendadas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntrevistasAgendadas_Candidaturas_CandidaturaId",
                table: "EntrevistasAgendadas",
                column: "CandidaturaId",
                principalTable: "Candidaturas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EntrevistasAgendadas_Candidaturas_CandidaturaId",
                table: "EntrevistasAgendadas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EntrevistasAgendadas",
                table: "EntrevistasAgendadas");

            migrationBuilder.RenameTable(
                name: "EntrevistasAgendadas",
                newName: "EntrevistaAgendada");

            migrationBuilder.RenameIndex(
                name: "IX_EntrevistasAgendadas_CandidaturaId",
                table: "EntrevistaAgendada",
                newName: "IX_EntrevistaAgendada_CandidaturaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntrevistaAgendada",
                table: "EntrevistaAgendada",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EntrevistaAgendada_Candidaturas_CandidaturaId",
                table: "EntrevistaAgendada",
                column: "CandidaturaId",
                principalTable: "Candidaturas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
