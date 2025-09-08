using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using System.Data;
using System.Text.Json;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Helpers;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eMonsterBusinessLayer : IDnd5eMonsterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _csvRepository;
        private readonly IMonster5eRepository _monster5ERepository;
        private readonly IMonsterBuildingGuidelineRepository _monsterBuildingGuidelineRepository;
        private readonly IOpenAiInterops _openAiInterops;

        public Dnd5eMonsterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository csvRepository, IMonster5eRepository monster5ERepository, IMonsterBuildingGuidelineRepository monsterBuildingGuidelineRepository, IOpenAiInterops openAiInterops)
        {
            _csvRepository = csvRepository;
            _monster5ERepository = monster5ERepository;
            _monsterBuildingGuidelineRepository = monsterBuildingGuidelineRepository;
            _openAiInterops = openAiInterops;
        }

        public async Task<Result<Monster5eDto>> CreateOriginalDndMonster(float challengeRating)
            => await Safe.ExecuteAsync(async () =>
            {
                //Pick a base monster from dnd2024
                var baseMonsters = await _monster5ERepository.FindByCrAsync(challengeRating);
                if (baseMonsters.IsFailure)
                    return Result<Monster5eDto>.Failure("No Monsters found for the given CR", ReasonType.NotFound);
                var randomSelector = new RandomSelector<Monster5eDbEntity>();
                var baseMonster = randomSelector.SelectOneRandomly(baseMonsters.Data.ToArray());

                CombatRoleEnum randomRole;
                do
                {
                    randomRole = RoleDescriptionsHelper.GetOneRandomRole();
                } while (baseMonster.Role.Value == randomRole);

                var role = RoleDescriptionsHelper.GetOneRandomRole();
                var roleDescription = RoleDescriptionsHelper.GetRoleDescription(role);

                //Generate monster lore from base monster
                var originalLore = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a original lore for {baseMonster.Name} that has the role {role} : {roleDescription}. Create what this {role} variation has more, what set it appart !");
                var originalName = await _openAiInterops.GetChatGptResponseAsync($"Take the base name of {baseMonster.Name} and use it to create a original name for a monster that has the role {role} : {roleDescription} and this lore {originalLore}");

                var originalManner = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a very short (10 to 15 words) manner description for {baseMonster.Name} that has the role {role} : {roleDescription}.");

                //TODO : Fix ChatGPT response sometimes not being a valid JSON  
                //Create Action related to the role of the monster
                var roleBasedActions = await _openAiInterops.ChatGptResponseAsync(CreateRoleActionPrompt(baseMonster, role, originalLore, roleDescription), responseFormat: OpenAIResponseFormatEnum.Json);

                var newMonster = new Monster5eDbEntity
                {
                    Actions = baseMonster.Actions,
                    Alignment = baseMonster.Alignment,
                    ArmorClass = baseMonster.ArmorClass,
                    ChallengeRating = baseMonster.ChallengeRating,
                    Climb = baseMonster.Climb,
                    Constitution = baseMonster.Constitution,
                    CreatureSize = baseMonster.CreatureSize,
                    CreatureSubType = baseMonster.CreatureSubType,
                    CreatureType = baseMonster.CreatureType,
                    Cr = baseMonster.Cr,
                    Equipments = baseMonster.Equipments,
                    HitDice = baseMonster.HitDice,
                    HitPoints = baseMonster.HitPoints,
                    InitiativeBonus = baseMonster.InitiativeBonus,
                    Intelligence = baseMonster.Intelligence,
                    Languages = baseMonster.Languages,
                    Lore = JsonSerializer.Serialize(new { originalLore }),
                    Manner = originalManner,
                    MinionArmorClass = baseMonster.MinionArmorClass,
                    MonsterGroup = baseMonster.MonsterGroup,
                    Name = originalName,
                    PageSource = baseMonster.PageSource,
                    ProficiencyBonus = baseMonster.ProficiencyBonus,
                    Role = role,
                    Skills = baseMonster.Skills,
                    Source = "AI Generated",
                    Speed = baseMonster.Speed,
                    Strength = baseMonster.Strength,
                    Swim = baseMonster.Swim,
                    Traits = baseMonster.Traits,
                    Wisdom = baseMonster.Wisdom,
                    Xp = baseMonster.Xp,
                    Charisma = baseMonster.Charisma,
                    DamageImmunities = baseMonster.DamageImmunities,
                    DamageResistances = baseMonster.DamageResistances,
                    Senses = baseMonster.Senses,
                    CrInLair = baseMonster.CrInLair,
                    DexSavingThrow = baseMonster.DexSavingThrow,
                    StrSavingThrow = baseMonster.StrSavingThrow,
                    ConSavingThrow = baseMonster.ConSavingThrow,
                    IntSavingThrow = baseMonster.IntSavingThrow,
                    WisSavingThrow = baseMonster.WisSavingThrow,
                    ChaSavingThrow = baseMonster.ChaSavingThrow,
                    Fly = baseMonster.Fly,
                    Dexterity = baseMonster.Dexterity,
                    Habitats = baseMonster.Habitats,
                    Symbarum5e = baseMonster.Symbarum5e
                };

                var newAction = JsonSerializer.Deserialize<ActionDto>(roleBasedActions, options: new JsonSerializerOptions() { PropertyNameCaseInsensitive = true});
                newMonster.Actions.Add(newAction.ToDbEntity());

                //await _monster5ERepository.Insert(newMonster);

                return Result<Monster5eDto>.Success(newMonster.ToDto());
            });

        public async Task<Result<List<Monster5eDto>>> GetAll5eMonsters()
            => await Safe.ExecuteAsync(async () =>
            {
                var results = await _monster5ERepository.GetAllAsync();
                if (results.IsFailure)
                    return Result<List<Monster5eDto>>.Failure(results.Error!, results.ReasonType);
                if (results.Data is null || !results.Data.Any())
                    return Result<List<Monster5eDto>>.Failure("No data", ReasonType.NotFound);
                return Result<List<Monster5eDto>>.Success(results.Data.Select(m => m.ToDto()).ToList());
            });

        public async Task<Result<List<Dnd5eMonsterDto>>> GetAllMonsterStatsByCr(int cr)
            => await Safe.ExecuteAsync(async () =>
            {
                var results = _csvRepository.GetAllDnd5e2024MonsterStatsByCr(cr).Verify(r => r.IsNull());
                if (results.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(results.Error!, results.ReasonType);
                return results;
            });

        private static string CreateRoleActionPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription)
            => $@"You are a D&D 5e game designer. You MUST respond with ONLY valid JSON, no other text.

                MONSTER CONTEXT:
                - Name: {baseMonster.Name}
                - Challenge Rating: {baseMonster.ChallengeRating}
                - Combat Role: {role} ({roleDescription})
                - Lore: {originalLore}
                - Existing Actions: {string.Join("; ", baseMonster.Actions.Select(a => a.Description))}

                TASK: Create ONE special action that perfectly represents this monster's combat role.

                CRITICAL RULES:
                1. Output ONLY valid JSON - no markdown, no backticks, no explanations
                2. All fields MUST be present - use null for unused optional fields
                3. Use exact field names (case-sensitive)
                4. Use integers for all enum values, never strings

                REQUIRED JSON STRUCTURE:
                {{
                  ""id"":""00000000-0000-0000-0000-000000000000"",
                  ""MonsterId"":""00000000-0000-0000-0000-000000000000"",
                  ""name"": ""Action Name Here"",
                  ""type"": 0,
                  ""attackType"": 0,
                  ""description"": ""Full mechanical description"",
                  ""shortRange"": null,
                  ""longRange"": null,
                  ""attackBonus"": null,
                  ""damageBonus"": null,
                  ""damageDice"": null,
                  ""numberDamageDice"": null,
                  ""damageType"": null,
                  ""limitPerDay"": null,
                  ""isProhibitedForMinion"": false,
                  ""actionTrigger"": null,
                  ""advantageCondition"": null,
                  ""disadvantageCondition"": null
                }}

                ENUM MAPPINGS (USE NUMBERS ONLY):
                Type: 0=Action, 1=BonusAction, 2=Reaction, 3=LegendaryAction, 4=LairAction, 5=MythicAction
                AttackType: 0=None, 1=MeleeWeaponAttack, 2=RangedWeaponAttack, 3=MeleeSpellAttack, 4=RangedSpellAttack, 5=SavingThrow
                DamageDice: 0=d4, 1=d6, 2=d8, 3=d10, 4=d12, 5=d20, 6=d100
                DamageType: 0=Acid, 1=Bludgeoning, 2=Cold, 3=Fire, 4=Force, 5=Lightning, 6=Necrotic, 7=Piercing, 8=Poison, 9=Psychic, 10=Radiant, 11=Slashing, 12=Thunder

                FIELD DETAILS:
                - id: Use ""00000000-0000-0000-0000-000000000000"" for new actions
                - MonsterId: Use ""00000000-0000-0000-0000-000000000000"" for new actions   
                - name: The action's name
                - type: Action type as integer (usually 0 for normal action)
                - attackType: How the action works mechanically (integer)
                - description: Complete D&D 5e mechanical description including all effects, saves, conditions
                - shortRange: Range in feet as string (""5"", ""30"", ""Touch"") or null
                - longRange: Long range for ranged attacks (""120"", ""600"") or null
                - attackBonus: To-hit bonus as integer (+8, +12) or null
                - damageBonus: Flat damage modifier as integer or null
                - damageDice: Die type as integer enum or null
                - numberDamageDice: Number of dice to roll as integer or null
                - damageType: Damage type as integer enum or null
                - limitPerDay: Uses per day as integer (1, 3) or null for unlimited
                - isProhibitedForMinion: true if minions can't use this, otherwise false
                - actionTrigger: For reactions only - trigger condition as string or null
                - advantageCondition: When attacks have advantage as string or null
                - disadvantageCondition: When attacks have disadvantage as string or null

                EXAMPLE VALID OUTPUT:
                {{
                  ""name"": ""Thunderous Smite"",
                  ""type"": 0,
                  ""attackType"": 1,
                  ""description"": ""Melee Weapon Attack: +8 to hit, reach 10 ft., one target. Hit: 14 (2d8 + 5) bludgeoning damage plus 9 (2d8) thunder damage, and the target must succeed on a DC 15 Strength saving throw or be pushed 10 feet away and knocked prone."",
                  ""shortRange"": ""10"",
                  ""longRange"": null,
                  ""attackBonus"": 8,
                  ""damageBonus"": 5,
                  ""damageDice"": 2,
                  ""numberDamageDice"": 2,
                  ""damageType"": 1,
                  ""limitPerDay"": null,
                  ""isProhibitedForMinion"": false,
                  ""actionTrigger"": null,
                  ""advantageCondition"": null,
                  ""disadvantageCondition"": null
                }}

                Now create the action JSON:";
    }
} 