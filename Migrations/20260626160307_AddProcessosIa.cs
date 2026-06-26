using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessosIa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MensagemTendenciaRisco",
                table: "AnomaliasDetectadas");

            migrationBuilder.DropColumn(
                name: "MensagemTendenciaValor",
                table: "AnomaliasDetectadas");

            migrationBuilder.CreateTable(
                name: "ProcessosIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Area = table.Column<string>(type: "TEXT", nullable: true),
                    Criticidade = table.Column<string>(type: "TEXT", nullable: true),
                    Objetivo = table.Column<string>(type: "TEXT", nullable: true),
                    CondicaoNormal = table.Column<string>(type: "TEXT", nullable: true),
                    ConsequenciasFalha = table.Column<string>(type: "TEXT", nullable: true),
                    ProcedimentoRecomendado = table.Column<string>(type: "TEXT", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessosIa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessoTagsIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessoIaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    PapelDaTag = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessoTagsIa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessoTagsIa_ProcessosIa_ProcessoIaId",
                        column: x => x.ProcessoIaId,
                        principalTable: "ProcessosIa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessosIa_ClienteId_Nome",
                table: "ProcessosIa",
                columns: new[] { "ClienteId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessoTagsIa_ProcessoIaId",
                table: "ProcessoTagsIa",
                column: "ProcessoIaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessoTagsIa");

            migrationBuilder.DropTable(
                name: "ProcessosIa");

            migrationBuilder.AddColumn<string>(
                name: "MensagemTendenciaRisco",
                table: "AnomaliasDetectadas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MensagemTendenciaValor",
                table: "AnomaliasDetectadas",
                type: "TEXT",
                nullable: true);
        }
    }
}
