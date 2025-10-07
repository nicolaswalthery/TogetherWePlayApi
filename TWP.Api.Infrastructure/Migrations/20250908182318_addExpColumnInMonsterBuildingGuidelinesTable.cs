using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addExpColumnInMonsterBuildingGuidelinesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "exp",
                schema: "public",
                table: "monster_building_guidelines",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exp",
                schema: "public",
                table: "monster_building_guidelines");
        }
    }
}
