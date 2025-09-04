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

                //Create Action related to the role of the monster
                var roleBasedActions = await _openAiInterops.GetChatGptResponseAsync(CreateRoleActionPrompt(baseMonster, role, originalLore, roleDescription));

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

                var newAction = JsonSerializer.Deserialize<ActionDbEntity>(roleBasedActions);
                newMonster.Actions.Add(newAction);

                await _monster5ERepository.Insert(newMonster);

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
                if(results.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(results.Error!, results.ReasonType);
                return results;
            });

        private static string CreateRoleActionPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription)
            => $@"
You are a D&D 5e game designer. Create ONE special action for a monster based on the following context:

CONTEXT:
- Monster Name: {baseMonster.Name}
- Combat Role: {role} ({roleDescription})
- Original Lore: {originalLore}
- Reference Actions: {string.Join("; ", baseMonster.Actions.Select(a => a.Description).ToList())}

REQUIREMENTS:
1. The action should perfectly represent and iconify the monster's role
2. Use D&D 5e mechanics (damage dice, save DCs, conditions, etc.)
3. Balance it according to the monster's CR: {baseMonster.ChallengeRating}

IMPORTANT: Return ONLY a valid JSON object (no markdown, no explanation) with this EXACT structure:

{{
  ""name"": ""string"",
  ""actionType"": ""string"",
  ""description"": ""string"",
  ""attackBonus"": null,
  ""damage"": null,
  ""damageDice"": null,
  ""damageType"": null,
  ""saveDC"": null,
  ""saveType"": null,
  ""recharge"": null,
  ""range"": null,
  ""numberOfTargets"": null,
  ""limitedUse"": null,
  ""condition"": null,
  ""duration"": null,
  ""isLegendaryAction"": false,
  ""legendaryCost"": null
}}

FIELD SPECIFICATIONS:
- name: Action name (e.g., ""Devastating Charge"")
- actionType: One of [""Action"", ""BonusAction"", ""Reaction"", ""LegendaryAction"", ""LairAction""]
- description: Full mechanical description with all rules
- attackBonus: Number or null (e.g., 8)
- damage: Average damage number or null (e.g., 14)
- damageDice: Dice notation or null (e.g., ""2d8+5"")
- damageType: One of [""acid"", ""bludgeoning"", ""cold"", ""fire"", ""force"", ""lightning"", ""necrotic"", ""piercing"", ""poison"", ""psychic"", ""radiant"", ""slashing"", ""thunder""] or null
- saveDC: Number or null (e.g., 15)
- saveType: One of [""STR"", ""DEX"", ""CON"", ""INT"", ""WIS"", ""CHA""] or null
- recharge: One of [""Recharge 5-6"", ""Recharge 6"", ""1/Day"", ""3/Day""] or null
- range: String or null (e.g., ""30 feet"", ""Touch"", ""Self"")
- numberOfTargets: Number or null
- limitedUse: String or null (e.g., ""3/Day"", ""1/Short Rest"")
- condition: String or null (e.g., ""stunned"", ""frightened"", ""paralyzed"")
- duration: String or null (e.g., ""1 minute"", ""until the end of your next turn"")
- isLegendaryAction: Boolean
- legendaryCost: Number or null (1, 2, or 3)

Return ONLY the JSON object, no other text.";
    }
} 