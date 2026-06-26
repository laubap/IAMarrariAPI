using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTagContextoIa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoTag",
                table: "TagsIaConfig");

            migrationBuilder.CreateTable(
                name: "TagsContextoIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    TipoRepresentacao = table.Column<string>(type: "TEXT", nullable: true),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    Criticidade = table.Column<string>(type: "TEXT", nullable: true),
                    Impactos = table.Column<string>(type: "TEXT", nullable: true),
                    TagsRelacionadas = table.Column<string>(type: "TEXT", nullable: true),
                    ModosOperacao = table.Column<string>(type: "TEXT", nullable: true),
                    CausasProvaveis = table.Column<string>(type: "TEXT", nullable: true),
                    AcoesRecomendadas = table.Column<string>(type: "TEXT", nullable: true),
                    Equipamento = table.Column<string>(type: "TEXT", nullable: true),
                    Area = table.Column<string>(type: "TEXT", nullable: true),
                    ObservacaoModoOperacao = table.Column<string>(type: "TEXT", nullable: true),
                    ContextoCompleto = table.Column<bool>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagsContextoIa", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagsContextoIa_ClienteId_TagName",
                table: "TagsContextoIa",
                columns: new[] { "ClienteId", "TagName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagsContextoIa");

            migrationBuilder.AddColumn<string>(
                name: "TipoTag",
                table: "TagsIaConfig",
                type: "TEXT",
                nullable: true);
        }
    }
}
