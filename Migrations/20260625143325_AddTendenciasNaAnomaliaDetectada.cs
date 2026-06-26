using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTendenciasNaAnomaliaDetectada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "TendenciaRisco",
                table: "AnomaliasDetectadas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TendenciaValor",
                table: "AnomaliasDetectadas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MensagemTendenciaRisco",
                table: "AnomaliasDetectadas");

            migrationBuilder.DropColumn(
                name: "MensagemTendenciaValor",
                table: "AnomaliasDetectadas");

            migrationBuilder.DropColumn(
                name: "TendenciaRisco",
                table: "AnomaliasDetectadas");

            migrationBuilder.DropColumn(
                name: "TendenciaValor",
                table: "AnomaliasDetectadas");
        }
    }
}
