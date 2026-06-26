using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTagDependenciasIa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagDependenciasIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    TagDependente = table.Column<string>(type: "TEXT", nullable: false),
                    TipoRelacao = table.Column<string>(type: "TEXT", nullable: false),
                    Impacto = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagDependenciasIa", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagDependenciasIa_ClienteId_TagName_TagDependente",
                table: "TagDependenciasIa",
                columns: new[] { "ClienteId", "TagName", "TagDependente" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagDependenciasIa");
        }
    }
}
