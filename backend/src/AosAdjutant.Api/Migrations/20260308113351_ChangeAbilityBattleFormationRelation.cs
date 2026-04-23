using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAbilityBattleFormationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_battle_formation_ability_ability_id",
                table: "battle_formation"
            );

            migrationBuilder.DropIndex(
                name: "IX_battle_formation_ability_id",
                table: "battle_formation"
            );

            migrationBuilder.AddColumn<bool>(
                name: "is_generic",
                table: "ability",
                type: "boolean",
                nullable: false,
                defaultValue: false
            );

            migrationBuilder.CreateTable(
                name: "battle_formation_ability",
                columns: table => new
                {
                    ability_id = table.Column<int>(type: "integer", nullable: false),
                    battle_formation_id = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_battle_formation_ability",
                        x => new { x.ability_id, x.battle_formation_id }
                    );
                    table.ForeignKey(
                        name: "FK_battle_formation_ability_ability_ability_id",
                        column: x => x.ability_id,
                        principalTable: "ability",
                        principalColumn: "ability_id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_battle_formation_ability_battle_formation_battle_formation_~",
                        column: x => x.battle_formation_id,
                        principalTable: "battle_formation",
                        principalColumn: "battle_formation_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_ability_ability_id",
                table: "battle_formation_ability",
                column: "ability_id",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_ability_battle_formation_id",
                table: "battle_formation_ability",
                column: "battle_formation_id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "battle_formation_ability");

            migrationBuilder.DropColumn(name: "is_generic", table: "ability");

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_ability_id",
                table: "battle_formation",
                column: "ability_id",
                unique: true
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
    }
}
