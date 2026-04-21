using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBattleFormationUK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_battle_formation_faction_id",
                table: "battle_formation"
            );

            migrationBuilder.DropIndex(name: "IX_battle_formation_name", table: "battle_formation");

            migrationBuilder.CreateIndex(
                name: "IX_battle_formation_faction_id_name",
                table: "battle_formation",
                columns: new[] { "faction_id", "name" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_battle_formation_faction_id_name",
                table: "battle_formation"
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
        }
    }
}
