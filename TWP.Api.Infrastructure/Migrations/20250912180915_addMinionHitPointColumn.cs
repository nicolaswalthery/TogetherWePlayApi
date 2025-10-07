using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addMinionHitPointColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "minion_hit_points",
                schema: "public",
                table: "monsters",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "minion_hit_points",
                schema: "public",
                table: "monsters");
        }
    }
}
