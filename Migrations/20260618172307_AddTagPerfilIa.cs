using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTagPerfilIa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerfisIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<string>(type: "TEXT", nullable: false),
                    TagName = table.Column<string>(type: "TEXT", nullable: false),
                    Media = table.Column<double>(type: "REAL", nullable: false),
                    DesvioPadrao = table.Column<double>(type: "REAL", nullable: false),
                    Minimo = table.Column<double>(type: "REAL", nullable: false),
                    Maximo = table.Column<double>(type: "REAL", nullable: false),
                    DataTreinamento = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisIa", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PerfisIa_ClienteId_TagName",
                table: "PerfisIa",
                columns: new[] { "ClienteId", "TagName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PerfisIa");
        }
    }
}
