using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CromosList.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "coleccion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_coleccion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "edicion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_edicion", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "editorial",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_editorial", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "equipo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tipo_cromo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_cromo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "publicacion",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    temporada = table.Column<string>(type: "text", nullable: false),
                    nombre = table.Column<string>(type: "text", nullable: true),
                    fecha_publicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    edicion_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publicacion", x => x.id);
                    table.ForeignKey(
                        name: "FK_publicacion_edicion_edicion_id",
                        column: x => x.edicion_id,
                        principalTable: "edicion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    temporada = table.Column<string>(type: "text", nullable: false),
                    editorial_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album", x => x.id);
                    table.ForeignKey(
                        name: "FK_album_editorial_editorial_id",
                        column: x => x.editorial_id,
                        principalTable: "editorial",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "jugador",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre = table.Column<string>(type: "text", nullable: false),
                    nombre_completo = table.Column<string>(type: "text", nullable: false),
                    equipo_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jugador", x => x.id);
                    table.ForeignKey(
                        name: "FK_jugador_equipo_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_equipo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    album_id = table.Column<long>(type: "bigint", nullable: false),
                    equipo_id = table.Column<long>(type: "bigint", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_equipo", x => x.id);
                    table.ForeignKey(
                        name: "FK_album_equipo_album_album_id",
                        column: x => x.album_id,
                        principalTable: "album",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_album_equipo_equipo_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "album_tipo_cromo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    album_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_cromo_id = table.Column<long>(type: "bigint", nullable: false),
                    orden = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_album_tipo_cromo", x => x.id);
                    table.ForeignKey(
                        name: "FK_album_tipo_cromo_album_album_id",
                        column: x => x.album_id,
                        principalTable: "album",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_album_tipo_cromo_tipo_cromo_tipo_cromo_id",
                        column: x => x.tipo_cromo_id,
                        principalTable: "tipo_cromo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cromo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    numero = table.Column<string>(type: "text", nullable: false),
                    jugador_id = table.Column<long>(type: "bigint", nullable: false),
                    equipo_id = table.Column<long>(type: "bigint", nullable: false),
                    edicion_id = table.Column<long>(type: "bigint", nullable: false),
                    publicacion_id = table.Column<long>(type: "bigint", nullable: false),
                    tipo_cromo_id = table.Column<long>(type: "bigint", nullable: false),
                    coleccion_id = table.Column<long>(type: "bigint", nullable: false),
                    album_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cromo", x => x.id);
                    table.ForeignKey(
                        name: "FK_cromo_album_album_id",
                        column: x => x.album_id,
                        principalTable: "album",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cromo_coleccion_coleccion_id",
                        column: x => x.coleccion_id,
                        principalTable: "coleccion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cromo_edicion_edicion_id",
                        column: x => x.edicion_id,
                        principalTable: "edicion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cromo_equipo_equipo_id",
                        column: x => x.equipo_id,
                        principalTable: "equipo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cromo_jugador_jugador_id",
                        column: x => x.jugador_id,
                        principalTable: "jugador",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cromo_publicacion_publicacion_id",
                        column: x => x.publicacion_id,
                        principalTable: "publicacion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_cromo_tipo_cromo_tipo_cromo_id",
                        column: x => x.tipo_cromo_id,
                        principalTable: "tipo_cromo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario_cromo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<long>(type: "bigint", nullable: false),
                    cromo_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_cromo", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_cromo_cromo_cromo_id",
                        column: x => x.cromo_id,
                        principalTable: "cromo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_cromo_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_album_editorial_id",
                table: "album",
                column: "editorial_id");

            migrationBuilder.CreateIndex(
                name: "IX_album_equipo_album_id",
                table: "album_equipo",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "IX_album_equipo_equipo_id",
                table: "album_equipo",
                column: "equipo_id");

            migrationBuilder.CreateIndex(
                name: "IX_album_tipo_cromo_album_id",
                table: "album_tipo_cromo",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "IX_album_tipo_cromo_tipo_cromo_id",
                table: "album_tipo_cromo",
                column: "tipo_cromo_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_album_id",
                table: "cromo",
                column: "album_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_coleccion_id",
                table: "cromo",
                column: "coleccion_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_edicion_id",
                table: "cromo",
                column: "edicion_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_equipo_id",
                table: "cromo",
                column: "equipo_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_jugador_id",
                table: "cromo",
                column: "jugador_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_publicacion_id",
                table: "cromo",
                column: "publicacion_id");

            migrationBuilder.CreateIndex(
                name: "IX_cromo_tipo_cromo_id",
                table: "cromo",
                column: "tipo_cromo_id");

            migrationBuilder.CreateIndex(
                name: "IX_jugador_equipo_id",
                table: "jugador",
                column: "equipo_id");

            migrationBuilder.CreateIndex(
                name: "IX_publicacion_edicion_id",
                table: "publicacion",
                column: "edicion_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_cromo_cromo_id",
                table: "usuario_cromo",
                column: "cromo_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_cromo_usuario_id",
                table: "usuario_cromo",
                column: "usuario_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "album_equipo");

            migrationBuilder.DropTable(
                name: "album_tipo_cromo");

            migrationBuilder.DropTable(
                name: "usuario_cromo");

            migrationBuilder.DropTable(
                name: "cromo");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "album");

            migrationBuilder.DropTable(
                name: "coleccion");

            migrationBuilder.DropTable(
                name: "jugador");

            migrationBuilder.DropTable(
                name: "publicacion");

            migrationBuilder.DropTable(
                name: "tipo_cromo");

            migrationBuilder.DropTable(
                name: "editorial");

            migrationBuilder.DropTable(
                name: "equipo");

            migrationBuilder.DropTable(
                name: "edicion");
        }
    }
}
