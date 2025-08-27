using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveTraitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "attack_bonus",
                schema: "public",
                table: "traits",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "damage_bonus",
                schema: "public",
                table: "traits",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "damage_dice",
                schema: "public",
                table: "traits",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "damage_type",
                schema: "public",
                table: "traits",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_damage_dice",
                schema: "public",
                table: "traits",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attack_bonus",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "damage_bonus",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "damage_dice",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "damage_type",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "number_damage_dice",
                schema: "public",
                table: "traits");
        }
    }
}
