using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addColumnCrAndCrInLair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cr",
                schema: "public",
                table: "monsters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cr_in_lair",
                schema: "public",
                table: "monsters",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cr",
                schema: "public",
                table: "monsters");

            migrationBuilder.DropColumn(
                name: "cr_in_lair",
                schema: "public",
                table: "monsters");
        }
    }
}
