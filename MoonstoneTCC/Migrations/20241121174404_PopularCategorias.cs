using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoonstoneTCC.Migrations
{
    /// <inheritdoc />
    public partial class PopularCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Ação', 'Jogos focados em combates, aventuras e adrenalina.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('RPG de Ação', 'Jogos que combinam elementos de RPG com combates dinâmicos.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Esportes', 'Simulações realistas de esportes populares, como futebol e basquete.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Corrida', 'Jogos emocionantes de carros e outros veículos, com alta velocidade.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('FPS', 'Jogos de tiro em primeira pessoa com combates intensos.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Mundo Aberto', 'Jogos com vastos ambientes para exploração e liberdade de escolhas.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Terror', 'Jogos sombrios e assustadores que oferecem experiências intensas.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Aventura', 'Jogos com narrativas ricas e exploração envolvente.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Luta', 'Jogos com combates corpo a corpo ou artes marciais épicas.')");

            migrationBuilder.Sql("INSERT INTO Categorias(CategoriaNome, Descricao) " +
                "VALUES('Estratégia', 'Jogos que exigem planejamento, raciocínio lógico e tomada de decisões.')");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Categorias");    
        }
    }
}
