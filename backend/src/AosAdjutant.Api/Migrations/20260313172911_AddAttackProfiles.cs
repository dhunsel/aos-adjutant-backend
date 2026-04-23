using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAttackProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attack_profile",
                columns: table => new
                {
                    attack_profile_id = table
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
                    is_ranged = table.Column<bool>(type: "boolean", nullable: false),
                    range = table.Column<int>(type: "integer", nullable: true),
                    attacks = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    to_hit = table.Column<int>(type: "integer", nullable: false),
                    to_wound = table.Column<int>(type: "integer", nullable: false),
                    rend = table.Column<int>(type: "integer", nullable: true),
                    damage = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
                    ),
                    unit_id = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attack_profile", x => x.attack_profile_id);
                    table.ForeignKey(
                        name: "FK_attack_profile_unit_unit_id",
                        column: x => x.unit_id,
                        principalTable: "unit",
                        principalColumn: "unit_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "weapon_effect",
                columns: table => new
                {
                    weapon_effect_id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    key = table.Column<string>(
                        type: "character varying(250)",
                        maxLength: 250,
                        nullable: false
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
                    table.PrimaryKey("PK_weapon_effect", x => x.weapon_effect_id);
                }
            );

            migrationBuilder.CreateTable(
                name: "attack_profile_weapon_effect",
                columns: table => new
                {
                    attack_profile_id = table.Column<int>(type: "integer", nullable: false),
                    weapon_effect_id = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_attack_profile_weapon_effect",
                        x => new { x.attack_profile_id, x.weapon_effect_id }
                    );
                    table.ForeignKey(
                        name: "FK_attack_profile_weapon_effect_attack_profile_attack_profile_~",
                        column: x => x.attack_profile_id,
                        principalTable: "attack_profile",
                        principalColumn: "attack_profile_id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_attack_profile_weapon_effect_weapon_effect_weapon_effect_id",
                        column: x => x.weapon_effect_id,
                        principalTable: "weapon_effect",
                        principalColumn: "weapon_effect_id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_attack_profile_name_unit_id",
                table: "attack_profile",
                columns: new[] { "name", "unit_id" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_attack_profile_unit_id",
                table: "attack_profile",
                column: "unit_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_attack_profile_weapon_effect_weapon_effect_id",
                table: "attack_profile_weapon_effect",
                column: "weapon_effect_id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_weapon_effect_key",
                table: "weapon_effect",
                column: "key",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_weapon_effect_name",
                table: "weapon_effect",
                column: "name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "attack_profile_weapon_effect");

            migrationBuilder.DropTable(name: "attack_profile");

            migrationBuilder.DropTable(name: "weapon_effect");
        }
    }
}
