using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRiscoProcessoNaAnomalia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassificacaoRisco",
                table: "AnomaliasDetectadas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreRiscoProcesso",
                table: "AnomaliasDetectadas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassificacaoRisco",
                table: "AnomaliasDetectadas");

            migrationBuilder.DropColumn(
                name: "ScoreRiscoProcesso",
                table: "AnomaliasDetectadas");
        }
    }
}
