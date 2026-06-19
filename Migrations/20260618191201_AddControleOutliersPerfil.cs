using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddControleOutliersPerfil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalOutliersRemovidos",
                table: "PerfisIa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRegistrosHistorico",
                table: "PerfisIa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalRegistrosUsados",
                table: "PerfisIa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalOutliersRemovidos",
                table: "PerfisIa");

            migrationBuilder.DropColumn(
                name: "TotalRegistrosHistorico",
                table: "PerfisIa");

            migrationBuilder.DropColumn(
                name: "TotalRegistrosUsados",
                table: "PerfisIa");
        }
    }
}
