using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using System.Data;
using System.Text.Json;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Core.Interface.Infrastructure;
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

                var roleDescription = RoleDescriptionsHelper.GetRoleDescription(randomRole);

                //Generate monster lore from base monster
                var originalLore = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a original lore for {baseMonster.Name} that has the role {randomRole} : {roleDescription}. Create what this {randomRole} variation has more, what set it appart !");
                var originalName = await _openAiInterops.GetChatGptResponseAsync($"Take the base name of {baseMonster.Name} and use it to create a original name for a monster that has the role {randomRole} : {roleDescription} and this lore {originalLore}");

                var originalManner = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a very short (10 to 15 words) manner description for {baseMonster.Name} that has the role {randomRole} : {roleDescription}.");

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
                    Role = randomRole,
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

                var monster5eRoleAdapterHelpers = new Monster5eRoleAdapterHelpers();
                var roleApplied = monster5eRoleAdapterHelpers.AdaptMonsterToRole(newMonster);

                if (randomRole != CombatRoleEnum.Minion)
                {
                    //Create Action related to the role of the monster
                    var actionType = EnumExtensions.GetRandomElementOfEnum<ActionTypeEnum>();
                    var attackType = EnumExtensions.GetRandomElementOfEnum<AttackTypeEnum>();

                    string roleBasedActions = String.Empty;
                    if (randomRole != CombatRoleEnum.Minion)
                        roleBasedActions = await _openAiInterops.ChatGptResponseAsync(CreateRoleActionPrompt(baseMonster, randomRole, originalLore, roleDescription, actionType, attackType));

                    var newAction = new ActionDto
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = roleApplied.modifiedMonster.Id,
                        Name = $"{randomRole} Special Attack",
                        Type = actionType.ToString(),
                        AttackType = attackType.ToString(),
                        Description = roleBasedActions,
                        IsProhibitedForMinion = false
                    };

                    newMonster.Actions.Add(newAction.ToDbEntity());
                }
                else //Minion cannot have actions
                    newMonster.Actions = null;

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

        private static string CreateRoleActionPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription, ActionTypeEnum actionType, AttackTypeEnum attackTypeEnum)
            => $@"You are a D&D 5e game designer. You MUST respond with ONLY the action description text, no other formatting or explanation.

                MONSTER CONTEXT:
                - Name: {baseMonster.Name}
                - Challenge Rating: {baseMonster.ChallengeRating}
                - Combat Role: {role} ({roleDescription})
                - Lore: {originalLore}
                - Stats: STR {baseMonster.Strength}, DEX {baseMonster.Dexterity}, CON {baseMonster.Constitution}, INT {baseMonster.Intelligence}, WIS {baseMonster.Wisdom}, CHA {baseMonster.Charisma}
                - Existing Actions: {string.Join("; ", baseMonster.Actions.Select(a => a.Description))}

                TASK: Create ONE special action description that perfectly represents this monster's combat role.

                REQUIREMENTS:
                - Write a complete D&D 5e mechanical description
                - Include attack type, to-hit bonus, reach/range, and damage as appropriate
                - Include any saving throws, DCs, and conditions
                - Follow standard D&D 5e formatting conventions
                - Make the action strongly reflect the monster's combat role
                - Output ONLY the description text, nothing else
                - The Action must be of type : {actionType}
                - The action is of attack type : {attackTypeEnum} which can be None, Melee, Ranged, MeleeOrRanged. If None, the action is not an attack. If Melee, the action is a melee attack. If Ranged, the action is a ranged attack. If MeleeOrRanged, the action can be either a melee or ranged attack.

                EXAMPLE OUTPUT:
                Melee Weapon Attack: +8 to hit, reach 10 ft., one target. Hit: 14 (2d8 + 5) bludgeoning damage plus 9 (2d8) thunder damage, and the target must succeed on a DC 15 Strength saving throw or be pushed 10 feet away and knocked prone.

                Now create the action description:";
    }
} 