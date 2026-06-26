using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAnomaliaDependenciaDetectada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnomaliaDependenciasDetectadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AnomaliaDetectadaId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagDependente = table.Column<string>(type: "TEXT", nullable: false),
                    ValorAtual = table.Column<double>(type: "REAL", nullable: false),
                    Media = table.Column<double>(type: "REAL", nullable: false),
                    LimiteInferior = table.Column<double>(type: "REAL", nullable: false),
                    LimiteSuperior = table.Column<double>(type: "REAL", nullable: false),
                    EhAnomalia = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    TipoRelacao = table.Column<string>(type: "TEXT", nullable: true),
                    Impacto = table.Column<string>(type: "TEXT", nullable: true),
                    DescricaoRelacao = table.Column<string>(type: "TEXT", nullable: true),
                    DataDeteccao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnomaliaDependenciasDetectadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnomaliaDependenciasDetectadas_AnomaliasDetectadas_AnomaliaDetectadaId",
                        column: x => x.AnomaliaDetectadaId,
                        principalTable: "AnomaliasDetectadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnomaliaDependenciasDetectadas_AnomaliaDetectadaId",
                table: "AnomaliaDependenciasDetectadas",
                column: "AnomaliaDetectadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnomaliaDependenciasDetectadas");
        }
    }
}
