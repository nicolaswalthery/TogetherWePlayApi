using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "advantage_condition",
                schema: "public",
                table: "traits",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disadvantage_condition",
                schema: "public",
                table: "traits",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trait_trigger",
                schema: "public",
                table: "traits",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creature_sub_type",
                schema: "public",
                table: "monsters",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "action_trigger",
                schema: "public",
                table: "actions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "advantage_condition",
                schema: "public",
                table: "actions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "disadvantage_condition",
                schema: "public",
                table: "actions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "long_range",
                schema: "public",
                table: "actions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "short_range",
                schema: "public",
                table: "actions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "advantage_condition",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "disadvantage_condition",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "trait_trigger",
                schema: "public",
                table: "traits");

            migrationBuilder.DropColumn(
                name: "creature_sub_type",
                schema: "public",
                table: "monsters");

            migrationBuilder.DropColumn(
                name: "action_trigger",
                schema: "public",
                table: "actions");

            migrationBuilder.DropColumn(
                name: "advantage_condition",
                schema: "public",
                table: "actions");

            migrationBuilder.DropColumn(
                name: "disadvantage_condition",
                schema: "public",
                table: "actions");

            migrationBuilder.DropColumn(
                name: "long_range",
                schema: "public",
                table: "actions");

            migrationBuilder.DropColumn(
                name: "short_range",
                schema: "public",
                table: "actions");
        }
    }
}
