using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TWP.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMonsterRelatedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "monsters",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    alignment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    challenge_rating = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    xp = table.Column<int>(type: "integer", nullable: false),
                    initiative_bonus = table.Column<int>(type: "integer", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creature_size = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    armor_class = table.Column<int>(type: "integer", nullable: false),
                    minion_armor_class = table.Column<int>(type: "integer", nullable: true),
                    hit_points = table.Column<int>(type: "integer", nullable: false),
                    hit_dice = table.Column<string>(type: "text", nullable: false),
                    speed = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    climb = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    swim = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fly = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    strength = table.Column<int>(type: "integer", nullable: true),
                    dexterity = table.Column<int>(type: "integer", nullable: true),
                    constitution = table.Column<int>(type: "integer", nullable: true),
                    intelligence = table.Column<int>(type: "integer", nullable: true),
                    wisdom = table.Column<int>(type: "integer", nullable: true),
                    charisma = table.Column<int>(type: "integer", nullable: true),
                    skills = table.Column<string>(type: "jsonb", nullable: true),
                    damage_immunities = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    senses = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    languages = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    con_saving_throw = table.Column<int>(type: "integer", nullable: true),
                    dex_saving_throw = table.Column<int>(type: "integer", nullable: true),
                    str_saving_throw = table.Column<int>(type: "integer", nullable: true),
                    wis_saving_throw = table.Column<int>(type: "integer", nullable: true),
                    cha_saving_throw = table.Column<int>(type: "integer", nullable: true),
                    int_saving_throw = table.Column<int>(type: "integer", nullable: true),
                    proficiency_bonus = table.Column<int>(type: "integer", nullable: true),
                    equipments = table.Column<string>(type: "jsonb", nullable: true),
                    habitats = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    creature_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    monster_group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    manner = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    lore = table.Column<string>(type: "jsonb", nullable: true),
                    page_source = table.Column<int>(type: "integer", nullable: false),
                    source = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_monsters", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "actions",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    monster_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    attack_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    attack_bonus = table.Column<int>(type: "integer", nullable: true),
                    damage_bonus = table.Column<int>(type: "integer", nullable: true),
                    damage_dice = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    number_damage_dice = table.Column<int>(type: "integer", nullable: true),
                    damage_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    limit_per_day = table.Column<int>(type: "integer", nullable: true),
                    is_prohibited_for_minion = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actions", x => x.id);
                    table.ForeignKey(
                        name: "FK_actions_monsters_monster_id",
                        column: x => x.monster_id,
                        principalSchema: "public",
                        principalTable: "monsters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "symbarum5es",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    monster_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shadow = table.Column<string>(type: "character varying(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_symbarum5es", x => x.id);
                    table.ForeignKey(
                        name: "FK_symbarum5es_monsters_monster_id",
                        column: x => x.monster_id,
                        principalSchema: "public",
                        principalTable: "monsters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "traits",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    monster_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_optional = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_traits", x => x.id);
                    table.ForeignKey(
                        name: "FK_traits_monsters_monster_id",
                        column: x => x.monster_id,
                        principalSchema: "public",
                        principalTable: "monsters",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_action_monster_id",
                schema: "public",
                table: "actions",
                column: "monster_id");

            migrationBuilder.CreateIndex(
                name: "IX_action_name",
                schema: "public",
                table: "actions",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_action_type",
                schema: "public",
                table: "actions",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_monster_challenge_rating",
                schema: "public",
                table: "monsters",
                column: "challenge_rating");

            migrationBuilder.CreateIndex(
                name: "IX_monster_creature_type",
                schema: "public",
                table: "monsters",
                column: "creature_type");

            migrationBuilder.CreateIndex(
                name: "IX_monster_group",
                schema: "public",
                table: "monsters",
                column: "monster_group");

            migrationBuilder.CreateIndex(
                name: "IX_monster_name",
                schema: "public",
                table: "monsters",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_symbarum5e_monster_id",
                schema: "public",
                table: "symbarum5es",
                column: "monster_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_feature_monster_id",
                schema: "public",
                table: "traits",
                column: "monster_id");

            migrationBuilder.CreateIndex(
                name: "IX_feature_title",
                schema: "public",
                table: "traits",
                column: "title");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "actions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "symbarum5es",
                schema: "public");

            migrationBuilder.DropTable(
                name: "traits",
                schema: "public");

            migrationBuilder.DropTable(
                name: "monsters",
                schema: "public");
        }
    }
}
