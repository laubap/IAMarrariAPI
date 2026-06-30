using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IAApi.Migrations
{
    /// <inheritdoc />
    public partial class RefatoraRelacionamentoProcessoEquipamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EquipamentosIa_ProcessosIa_ProcessoIaId",
                table: "EquipamentosIa");

            migrationBuilder.DropIndex(
                name: "IX_EquipamentosIa_ProcessoIaId",
                table: "EquipamentosIa");

            migrationBuilder.DropColumn(
                name: "ProcessoIaId",
                table: "EquipamentosIa");

            migrationBuilder.CreateTable(
                name: "ProcessoEquipamentosIa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcessoIaId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipamentoIaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PapelNoProcesso = table.Column<string>(type: "TEXT", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessoEquipamentosIa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessoEquipamentosIa_EquipamentosIa_EquipamentoIaId",
                        column: x => x.EquipamentoIaId,
                        principalTable: "EquipamentosIa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessoEquipamentosIa_ProcessosIa_ProcessoIaId",
                        column: x => x.ProcessoIaId,
                        principalTable: "ProcessosIa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessoEquipamentosIa_EquipamentoIaId",
                table: "ProcessoEquipamentosIa",
                column: "EquipamentoIaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessoEquipamentosIa_ProcessoIaId_EquipamentoIaId",
                table: "ProcessoEquipamentosIa",
                columns: new[] { "ProcessoIaId", "EquipamentoIaId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessoEquipamentosIa");

            migrationBuilder.AddColumn<int>(
                name: "ProcessoIaId",
                table: "EquipamentosIa",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EquipamentosIa_ProcessoIaId",
                table: "EquipamentosIa",
                column: "ProcessoIaId");

            migrationBuilder.AddForeignKey(
                name: "FK_EquipamentosIa_ProcessosIa_ProcessoIaId",
                table: "EquipamentosIa",
                column: "ProcessoIaId",
                principalTable: "ProcessosIa",
                principalColumn: "Id");
        }
    }
}
