using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CromosList.Migrations
{
    /// <inheritdoc />
    public partial class MakeCromoCategoriasOpcionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cromo_coleccion_coleccion_id",
                table: "cromo");

            migrationBuilder.DropForeignKey(
                name: "FK_cromo_publicacion_publicacion_id",
                table: "cromo");

            migrationBuilder.AlterColumn<long>(
                name: "publicacion_id",
                table: "cromo",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "coleccion_id",
                table: "cromo",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_cromo_coleccion_coleccion_id",
                table: "cromo",
                column: "coleccion_id",
                principalTable: "coleccion",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_cromo_publicacion_publicacion_id",
                table: "cromo",
                column: "publicacion_id",
                principalTable: "publicacion",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cromo_coleccion_coleccion_id",
                table: "cromo");

            migrationBuilder.DropForeignKey(
                name: "FK_cromo_publicacion_publicacion_id",
                table: "cromo");

            migrationBuilder.AlterColumn<long>(
                name: "publicacion_id",
                table: "cromo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "coleccion_id",
                table: "cromo",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cromo_coleccion_coleccion_id",
                table: "cromo",
                column: "coleccion_id",
                principalTable: "coleccion",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cromo_publicacion_publicacion_id",
                table: "cromo",
                column: "publicacion_id",
                principalTable: "publicacion",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
