using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "faction",
                columns: table => new
                {
                    faction_id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    name = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faction", x => x.faction_id);
                }
            );

            migrationBuilder.CreateTable(
                name: "battle_formation",
                columns: table => new
                {
                    battle_formation_id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    name = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    faction_id = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_battle_formation", x => x.battle_formation_id);
                    table.ForeignKey(
                        name: "FK_battle_formation_faction_faction_id",
                        column: x => x.faction_id,
                        principalTable: "faction",
                        principalColumn: "faction_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_faction_id",
                table: "battle_formation",
                column: "faction_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_name",
                table: "battle_formation",
                column: "name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_faction_name",
                table: "faction",
                column: "name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "battle_formation");

            migrationBuilder.DropTable(name: "faction");
        }
    }
}
