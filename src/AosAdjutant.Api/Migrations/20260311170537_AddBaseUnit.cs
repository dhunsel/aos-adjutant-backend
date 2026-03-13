using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ability_id",
                table: "battle_formation");

            migrationBuilder.CreateTable(
                name: "unit",
                columns: table => new
                {
                    unit_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    health = table.Column<int>(type: "integer", nullable: false),
                    move = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    save = table.Column<int>(type: "integer", nullable: false),
                    control = table.Column<int>(type: "integer", nullable: false),
                    ward_save = table.Column<int>(type: "integer", nullable: true),
                    faction_id = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit", x => x.unit_id);
                    table.ForeignKey(
                        name: "FK_unit_faction_faction_id",
                        column: x => x.faction_id,
                        principalTable: "faction",
                        principalColumn: "faction_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "unit_ability",
                columns: table => new
                {
                    ability_id = table.Column<int>(type: "integer", nullable: false),
                    unit_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit_ability", x => new { x.ability_id, x.unit_id });
                    table.ForeignKey(
                        name: "FK_unit_ability_ability_ability_id",
                        column: x => x.ability_id,
                        principalTable: "ability",
                        principalColumn: "ability_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_unit_ability_unit_unit_id",
                        column: x => x.unit_id,
                        principalTable: "unit",
                        principalColumn: "unit_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_unit_faction_id_name",
                table: "unit",
                columns: new[] { "faction_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_unit_ability_ability_id",
                table: "unit_ability",
                column: "ability_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_unit_ability_unit_id",
                table: "unit_ability",
                column: "unit_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unit_ability");

            migrationBuilder.DropTable(
                name: "unit");

            migrationBuilder.AddColumn<int>(
                name: "ability_id",
                table: "battle_formation",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
