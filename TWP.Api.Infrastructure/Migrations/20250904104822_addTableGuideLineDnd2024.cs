using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTableGuideLineDnd2024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "monster_building_guidelines",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cr = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    cr_numeric = table.Column<float>(type: "real", precision: 5, scale: 3, nullable: false),
                    proficiency_bonus = table.Column<int>(type: "integer", nullable: false),
                    armor_class = table.Column<int>(type: "integer", nullable: false),
                    min_hp = table.Column<int>(type: "integer", nullable: false),
                    max_hp = table.Column<int>(type: "integer", nullable: false),
                    average_hp = table.Column<int>(type: "integer", nullable: false),
                    hp_range = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tof_hp = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    attack_bonus = table.Column<int>(type: "integer", nullable: false),
                    multi_attack_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    average_damage_per_round = table.Column<int>(type: "integer", nullable: false),
                    total_damage_avg = table.Column<int>(type: "integer", nullable: false),
                    total_damage_legendary_avg = table.Column<int>(type: "integer", nullable: false),
                    damage_per_round = table.Column<int>(type: "integer", nullable: false),
                    damage_per_round_alt = table.Column<int>(type: "integer", nullable: false),
                    save_dc = table.Column<int>(type: "integer", nullable: false),
                    initiative_bonus = table.Column<int>(type: "integer", nullable: false),
                    experience_points = table.Column<int>(type: "integer", nullable: false),
                    example_monsters = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_official = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monster_building_guidelines", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_guideline_active_cr_numeric",
                schema: "public",
                table: "monster_building_guidelines",
                columns: new[] { "is_active", "cr_numeric" });

            migrationBuilder.CreateIndex(
                name: "IX_guideline_cr",
                schema: "public",
                table: "monster_building_guidelines",
                column: "cr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_guideline_cr_numeric",
                schema: "public",
                table: "monster_building_guidelines",
                column: "cr_numeric");

            migrationBuilder.CreateIndex(
                name: "IX_guideline_is_active",
                schema: "public",
                table: "monster_building_guidelines",
                column: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "monster_building_guidelines",
                schema: "public");
        }
    }
}
