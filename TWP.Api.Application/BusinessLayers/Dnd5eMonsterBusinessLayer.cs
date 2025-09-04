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
                if (results.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(results.Error!, results.ReasonType);
                return results;
            });

        private static string CreateRoleActionPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription)
            => "$@\"\r\nYou are a D&D 5e game designer. Create ONE special action for a monster based on the following context:\r\n\r\nCONTEXT:\r\n- Monster Name: {baseMonster.Name}\r\n- Combat Role: {role} ({roleDescription})\r\n- Original Lore: {originalLore}\r\n- Reference Actions: {string.Join(\"; \", baseMonster.Actions.Select(a => a.Description).ToList())}\r\n\r\nREQUIREMENTS:\r\n1. The action should perfectly represent and iconify the monster's role\r\n2. Use D&D 5e mechanics (damage dice, save DCs, conditions, etc.)\r\n3. Balance it according to the monster's CR: {baseMonster.ChallengeRating}\r\n\r\nIMPORTANT: Return ONLY a valid JSON object (no markdown, no explanation) with this EXACT structure:\r\n\r\n{{\r\n  \"\"name\"\": \"\"string\"\",\r\n  \"\"type\"\": 0,\r\n  \"\"attackType\"\": 0,\r\n  \"\"description\"\": \"\"string\"\",\r\n  \"\"shortRange\"\": null,\r\n  \"\"longRange\"\": null,\r\n  \"\"attackBonus\"\": null,\r\n  \"\"damageBonus\"\": null,\r\n  \"\"damageDice\"\": null,\r\n  \"\"numberDamageDice\"\": null,\r\n  \"\"damageType\"\": null,\r\n  \"\"limitPerDay\"\": null,\r\n  \"\"isProhibitedForMinion\"\": false,\r\n  \"\"actionTrigger\"\": null,\r\n  \"\"advantageCondition\"\": null,\r\n  \"\"disadvantageCondition\"\": null\r\n}}\r\n\r\nFIELD SPECIFICATIONS:\r\n- name: Action name (e.g., \"\"Devastating Charge\"\")\r\n- type: Integer enum - 0=Action, 1=BonusAction, 2=Reaction, 3=LegendaryAction, 4=LairAction, 5=MythicAction\r\n- attackType: Integer enum - 0=None, 1=MeleeWeaponAttack, 2=RangedWeaponAttack, 3=MeleeSpellAttack, 4=RangedSpellAttack, 5=SavingThrow\r\n- description: Full mechanical description with all rules and effects\r\n- shortRange: String or null (e.g., \"\"5\"\", \"\"30\"\", \"\"Touch\"\")\r\n- longRange: String or null (e.g., \"\"120\"\", \"\"600\"\") - only for ranged attacks\r\n- attackBonus: Integer or null (e.g., 8, 12)\r\n- damageBonus: Integer or null (e.g., 5, 8) - the flat damage bonus added to dice\r\n- damageDice: Integer enum or null - 0=d4, 1=d6, 2=d8, 3=d10, 4=d12, 5=d20, 6=d100\r\n- numberDamageDice: Integer or null (e.g., 2, 3, 4) - number of damage dice to roll\r\n- damageType: Integer enum or null - 0=Acid, 1=Bludgeoning, 2=Cold, 3=Fire, 4=Force, 5=Lightning, 6=Necrotic, 7=Piercing, 8=Poison, 9=Psychic, 10=Radiant, 11=Slashing, 12=Thunder\r\n- limitPerDay: Integer or null (e.g., 1, 3) - number of uses per day, null if unlimited\r\n- isProhibitedForMinion: Boolean - true if minions cannot use this action\r\n- actionTrigger: String or null - for reactions, what triggers it (e.g., \"\"when hit by an attack\"\", \"\"when an enemy ends its turn within 5 feet\"\")\r\n- advantageCondition: String or null - condition that grants advantage (e.g., \"\"against prone targets\"\", \"\"if the target is frightened\"\")\r\n- disadvantageCondition: String or null - condition that imposes disadvantage (e.g., \"\"in sunlight\"\", \"\"against targets wearing heavy armor\"\")\r\n\r\nReturn ONLY the JSON object, no other text.\"";
    }
} 