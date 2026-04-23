using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAbilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ability_id",
                table: "battle_formation",
                type: "integer",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.CreateTable(
                name: "ability",
                columns: table => new
                {
                    ability_id = table
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
                    reaction = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    declaration = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    effect = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    phase = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    restriction = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    turn = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: true
                    ),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ability", x => x.ability_id);
                }
            );

            migrationBuilder.CreateTable(
                name: "faction_ability",
                columns: table => new
                {
                    ability_id = table.Column<int>(type: "integer", nullable: false),
                    faction_id = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_faction_ability", x => new { x.ability_id, x.faction_id });
                    table.ForeignKey(
                        name: "FK_faction_ability_ability_ability_id",
                        column: x => x.ability_id,
                        principalTable: "ability",
                        principalColumn: "ability_id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_faction_ability_faction_faction_id",
                        column: x => x.faction_id,
                        principalTable: "faction",
                        principalColumn: "faction_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_ability_id",
                table: "battle_formation",
                column: "ability_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_faction_ability_ability_id",
                table: "faction_ability",
                column: "ability_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_faction_ability_faction_id",
                table: "faction_ability",
                column: "faction_id"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_battle_formation_ability_ability_id",
                table: "battle_formation",
                column: "ability_id",
                principalTable: "ability",
                principalColumn: "ability_id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_battle_formation_ability_ability_id",
                table: "battle_formation"
            );

            migrationBuilder.DropTable(name: "faction_ability");

            migrationBuilder.DropTable(name: "ability");

            migrationBuilder.DropIndex(
                name: "IX_battle_formation_ability_id",
                table: "battle_formation"
            );

            migrationBuilder.DropColumn(name: "ability_id", table: "battle_formation");
        }
    }
}
