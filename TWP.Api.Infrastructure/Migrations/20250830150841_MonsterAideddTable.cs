using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MonsterAideddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AideddMonsters",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CR = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<string>(type: "text", nullable: false),
                    AC = table.Column<int>(type: "integer", nullable: false),
                    HP = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<string>(type: "text", nullable: false),
                    Alignment = table.Column<string>(type: "text", nullable: false),
                    Legendary = table.Column<string>(type: "text", nullable: false),
                    Habitat = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AideddMonsters", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AideddMonsters",
                schema: "public");
        }
    }
}
