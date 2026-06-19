using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposPerfilTag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TipoTag",
                table: "TagsIaConfig",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<double>(
                name: "Amplitude",
                table: "PerfisIa",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PercentualZeros",
                table: "PerfisIa",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "QuantidadePicos",
                table: "PerfisIa",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "VariacaoMedia",
                table: "PerfisIa",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amplitude",
                table: "PerfisIa");

            migrationBuilder.DropColumn(
                name: "PercentualZeros",
                table: "PerfisIa");

            migrationBuilder.DropColumn(
                name: "QuantidadePicos",
                table: "PerfisIa");

            migrationBuilder.DropColumn(
                name: "VariacaoMedia",
                table: "PerfisIa");

            migrationBuilder.AlterColumn<string>(
                name: "TipoTag",
                table: "TagsIaConfig",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
