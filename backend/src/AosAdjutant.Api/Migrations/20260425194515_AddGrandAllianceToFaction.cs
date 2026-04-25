using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AosAdjutant.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGrandAllianceToFaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "grand_alliance",
                table: "faction",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                defaultValue: ""
            );

            migrationBuilder.Sql("UPDATE faction set grand_alliance = 'Order'");

            migrationBuilder.AlterColumn<string>(
                name: "grand_alliance",
                table: "faction",
                nullable: false
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "grand_alliance", table: "faction");
        }
    }
}
