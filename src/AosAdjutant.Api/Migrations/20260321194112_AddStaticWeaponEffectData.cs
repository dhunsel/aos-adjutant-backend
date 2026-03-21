using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticWeaponEffectData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "weapon_effect");

            migrationBuilder.InsertData(
                table: "weapon_effect",
                columns: new[] { "weapon_effect_id", "key", "name" },
                values: new object[,]
                {
                    { 1, "crit_2_hits", "Crit (2 Hits)" },
                    { 2, "crit_auto_wound", "Crit (Auto-wound)" },
                    { 3, "crit_mortal", "Crit (Mortal)" },
                    { 4, "charge_1_damage", "Charge (+1 Damage)" },
                    { 5, "companion", "Companion" },
                    { 6, "shoot_in_combat", "Shoot in Combat" },
                    { 7, "anti_monster", "Anti-Monster (+1 Rend)" },
                    { 8, "anti_hero", "Anti-Hero (+1 Rend)" },
                    { 9, "anti_infantry", "Anti-Infantry (+1 Rend)" },
                    { 10, "anti_cavalry", "Anti-Cavalry (+1 Rend)" },
                    { 11, "anti_wizard", "Anti-Wizard (+1 Rend)" },
                    { 12, "anti_manifestation", "Anti-Manifestation (+1 Rend)" },
                    { 13, "anti_charge", "Anti-Charge (+1 Rend)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "weapon_effect",
                keyColumn: "weapon_effect_id",
                keyValue: 13);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "weapon_effect",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }
    }
}
