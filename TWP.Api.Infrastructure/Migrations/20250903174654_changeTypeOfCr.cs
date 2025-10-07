using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeTypeOfCr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "cr_in_lair",
                schema: "public",
                table: "monsters",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "cr",
                schema: "public",
                table: "monsters",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "cr_in_lair",
                schema: "public",
                table: "monsters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "cr",
                schema: "public",
                table: "monsters",
                type: "integer",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
