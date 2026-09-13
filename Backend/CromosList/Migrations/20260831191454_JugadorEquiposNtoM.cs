using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CromosList.Migrations
{
    /// <inheritdoc />
    public partial class JugadorEquiposNtoM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jugador_equipo_equipo_id",
                table: "jugador");

            migrationBuilder.DropIndex(
                name: "IX_jugador_equipo_id",
                table: "jugador");

            migrationBuilder.DropColumn(
                name: "equipo_id",
                table: "jugador");

            migrationBuilder.CreateTable(
                name: "equipo_jugador",
                columns: table => new
                {
                    equipo_id = table.Column<long>(type: "bigint", nullable: false),
                    jugador_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipo_jugador", x => new { x.equipo_id, x.jugador_id });
                    table.ForeignKey(
                        name: "FK_equipo_jugador_equipo_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_equipo_jugador_jugador_jugador_id",
                        column: x => x.jugador_id,
                        principalTable: "jugador",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_equipo_jugador_jugador_id",
                table: "equipo_jugador",
                column: "jugador_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "equipo_jugador");

            migrationBuilder.AddColumn<long>(
                name: "equipo_id",
                table: "jugador",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_jugador_equipo_id",
                table: "jugador",
                column: "equipo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_jugador_equipo_equipo_id",
                table: "jugador",
                column: "equipo_id",
                principalTable: "equipo",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
