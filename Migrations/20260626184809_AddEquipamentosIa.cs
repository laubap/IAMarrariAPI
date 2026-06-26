using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipamentosIa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EquipamentosIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Area = table.Column<string>(type: "TEXT", nullable: true),
                    TipoEquipamento = table.Column<string>(type: "TEXT", nullable: true),
                    Criticidade = table.Column<string>(type: "TEXT", nullable: true),
                    Fabricante = table.Column<string>(type: "TEXT", nullable: true),
                    Modelo = table.Column<string>(type: "TEXT", nullable: true),
                    DataUltimaManutencao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", nullable: true),
                    ProcessoIaId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipamentosIa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipamentosIa_ProcessosIa_ProcessoIaId",
                        column: x => x.ProcessoIaId,
                        principalTable: "ProcessosIa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EquipamentoTagsIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EquipamentoIaId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    PapelDaTag = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipamentoTagsIa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquipamentoTagsIa_EquipamentosIa_EquipamentoIaId",
                        column: x => x.EquipamentoIaId,
                        principalTable: "EquipamentosIa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosIa_ClienteId_Nome",
                table: "EquipamentosIa",
                columns: new[] { "ClienteId", "Nome" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosIa_ProcessoIaId",
                table: "EquipamentosIa",
                column: "ProcessoIaId");

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentoTagsIa_EquipamentoIaId",
                table: "EquipamentoTagsIa",
                column: "EquipamentoIaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EquipamentoTagsIa");

            migrationBuilder.DropTable(
                name: "EquipamentosIa");
        }
    }
}
