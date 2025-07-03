using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Interops;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _dnd2024AllMonsterStatsCsvRepository;
        private readonly IDnd5eEncounterDataJsonRepository _dnd5ERelationBetweenXpAndCrJsonRepository;
        private readonly IOpenAiInterops _openAiInterops;

        public Dnd5eEncounterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository dnd2024AllMonsterStatsCsvRepository, IDnd5eEncounterDataJsonRepository dnd5ERelationBetweenXpAndCrJsonRepository, IOpenAiInterops openAiInterops)
        {
            _dnd2024AllMonsterStatsCsvRepository = dnd2024AllMonsterStatsCsvRepository;
            _dnd5ERelationBetweenXpAndCrJsonRepository = dnd5ERelationBetweenXpAndCrJsonRepository;
            _openAiInterops = openAiInterops;
        }

        public async Task<Result<Dnd5eEncounterGeneratedDto>> EncounterRandomGenerator(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            IList<MonsterHabitatEnum> monsterHabitats)
                => await Safe.ExecuteAsync(async () =>
                {
                    if(playerLevels.HasNoElement())
                        return Result<Dnd5eEncounterGeneratedDto>.Failure("No Elements", ReasonType.BadParameter);
                    var cr = playerLevels.Min();
                    var result = _dnd2024AllMonsterStatsCsvRepository.GetAllDnd5e2024MonsterStatsByCr(cr + 1).Where(m => m.Name.IsNotNullOrEmptyOrWhiteSpace()).ToList().Verify(data => data.IsNull());
                    if (result.IsFailure)
                        return Result<Dnd5eEncounterGeneratedDto>.Failure(result.Error!, result.ReasonType);

                    //Fix the fact that the csv do not have XP data for each monster. This code is not intended to stay.
                    var resultCrRelatedToXp = _dnd5ERelationBetweenXpAndCrJsonRepository.GetAll();
                    foreach (var datum in result.Data!.Where(d => d.XP.IsNullOrEmptyOrWhiteSpace()))
                        datum.XP = resultCrRelatedToXp.First(r => r.CR == datum.CR).XP.ToString();

                    var filteredMonsters = monsterHabitats.Contains(MonsterHabitatEnum.Any) ? result.Data : result.Data!.Where(d => monsterHabitats.Contains(d.Habitat.ToEnum())).ToList();

                    var expEncounterBudget = ComputeExpBudget(encounterDifficulty, playerLevels);

                    var encounterGenerated = GenerateEncounter(filteredMonsters!, expEncounterBudget, playerLevels.Count, playerLevels.Min());

                    var formattedEncounterData = GetFormattedEncounter(encounterDifficulty: encounterDifficulty, playerLevels: playerLevels, encounterNarrativeContext, monsterHabitats: monsterHabitats, filteredMonsters: filteredMonsters, expEncounterBudget: expEncounterBudget);

                    var openAiresult = await _openAiInterops.GetChatGptResponseAsync(
                        message: $"Create an Dnd5e Encounter using these data : Encounter data for you to use : {formattedEncounterData.Data} which were picked according to Difficulty : {encounterDifficulty.ToString()}, Number of players : {playerLevels.Count}, Party Level : {playerLevels.Min()}, NarrativeContext : {encounterNarrativeContext}, Monster Habitats : {string.Join(",", monsterHabitats.Select(h => h.ToString()))}",
                        systemPrompt: "");

                    var finaleResult = new Dnd5eEncounterGeneratedDto() 
                                        { 
                                            Cr = cr, EncounterDifficulty = encounterDifficulty, 
                                            EncounterNarrativeContext = encounterNarrativeContext, 
                                            MonsterHabitats = monsterHabitats, 
                                            Monsters = encounterGenerated, 
                                            OpenAiResponse = openAiresult,
                                            FormattedEncounterData = formattedEncounterData.Data!
                    };

                    return Result<Dnd5eEncounterGeneratedDto>.Success(finaleResult);
                });

        /// <summary>
        /// Gets formatted text representation of an encounter
        /// </summary>
        /// <param name="encounterDifficulty">Difficulty of the encounter</param>
        /// <param name="playerLevels">Player levels</param>
        /// <param name="encounterNarrativeContext">Narrative context</param>
        /// <param name="monsterHabitats">Monster habitats</param>
        /// <param name="formatType">Type of formatting (Full, Summary, or List)</param>
        /// <returns>Formatted text representation of the encounter</returns>
        public Result<string> GetFormattedEncounter(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            IList<MonsterHabitatEnum> monsterHabitats,
            IList<Dnd5eMonsterDto> filteredMonsters,
            int expEncounterBudget,
            string formatType = "Summary")
        {
            var encounterResult = GenerateEncounter(filteredMonsters!, expEncounterBudget, playerLevels.Count, playerLevels.Min());

            var formattedText = formatType.ToLower() switch
            {
                "full" => MonsterDataFormatter.FormatMonstersList(encounterResult!),
                "list" => MonsterDataFormatter.FormatMonstersList(encounterResult!),
                "summary" => MonsterDataFormatter.FormatMonstersSummary(encounterResult!),
                _ => MonsterDataFormatter.FormatMonstersSummary(encounterResult!)
            };

            return Result<string>.Success(formattedText);
        }

        /// <summary>
        /// Gets formatted text representation of a single monster
        /// </summary>
        /// <param name="monster">The monster to format</param>
        /// <returns>Formatted text representation of the monster</returns>
        public static string GetFormattedMonsterData(Dnd5eMonsterDto monster)
        {
            return MonsterDataFormatter.FormatMonsterData(monster);
        }

        /// <summary>
        /// Compute the experience budget for a party
        /// </summary>
        /// <returns></returns>
        private int ComputeExpBudget(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels)
        {
            if(playerLevels.Any(pl => pl <= 0 || pl > 20))
                return 0;

            // XP budget table as per the image (dnd2024 version)
            var xpTable = new Dictionary<int, (int Low, int Moderate, int High)>
            {
                {1, (50, 75, 100)},
                {2, (100, 150, 200)},
                {3, (150, 225, 400)},
                {4, (250, 375, 500)},
                {5, (500, 750, 1100)},
                {6, (600, 1000, 1400)},
                {7, (750, 1300, 1700)},
                {8, (1000, 1700, 2100)},
                {9, (1300, 2000, 2600)},
                {10, (1600, 2300, 3100)},
                {11, (1900, 2900, 4100)},
                {12, (2200, 3700, 4700)},
                {13, (2600, 4200, 5400)},
                {14, (2900, 4900, 6200)},
                {15, (3300, 5400, 7800)},
                {16, (3800, 6100, 9800)},
                {17, (4500, 7200, 11700)},
                {18, (5000, 8700, 14200)},
                {19, (5500, 10700, 17200)},
                {20, (6400, 13200, 22000)}
            };

            int total = 0;
            foreach (var level in playerLevels)
            {
                var xp = encounterDifficulty switch
                {
                    EncounterDifficultyEnum.Low => xpTable[level].Low,
                    EncounterDifficultyEnum.Moderate => xpTable[level].Moderate,
                    EncounterDifficultyEnum.High => xpTable[level].High,
                    _ => throw new ArgumentException("Unsupported difficulty")
                };
                total += xp;
            }
            return total;
        }

        /// <summary>
        /// Generates a single encounter by selecting monsters whose total XP does not exceed the given budget.
        /// </summary>
        /// <param name="monsters">List of available monsters.</param>
        /// <param name="expEncounterBudget">Total XP budget for the encounter.</param>
        /// <returns>List of monsters for the encounter.</returns>
        private List<Dnd5eMonsterDto> GenerateEncounter(IList<Dnd5eMonsterDto> monsters, int expEncounterBudget, int playerNumber, int partyLevel)
        {
            var encounter = new List<Dnd5eMonsterDto>();
            var remainingBudget = expEncounterBudget;
            var maxMonsters = playerNumber * 2;
            var maxDifferentMonster = 4;

            var computedMaxDifferentMonster = new Dice(1, maxDifferentMonster).Roll;
            // Group monsters by type 
            var monstersGroup = monsters.Where(m => m.SanitizedXp <= expEncounterBudget).GroupBy(m => m.Type)
                                        .OrderBy(_ => Guid.NewGuid()) // Shuffle types
                                        .Take(1).First();

            // Flatten the selected groups, but keep track of type
            var selectedAvailableMonsters = monstersGroup.Select(m => m).OrderBy(_ => Guid.NewGuid()).Take(computedMaxDifferentMonster).ToList();

            for(var i = 0; i < selectedAvailableMonsters.ToArray().Length && remainingBudget > 9/*10 is the minimum xp budget available for a dnd monster*/ && encounter.Count < maxMonsters; i++)
            {
                var monster = selectedAvailableMonsters[i];
                if (monster.SanitizedXp <= remainingBudget && encounter.Count < maxMonsters)
                {
                    encounter.Add(monster);
                    remainingBudget -= monster.SanitizedXp;
                }
                else  // If we have not reached the max number of monsters and have remaining budget, try to add one last monster that fits the budget
                {
                    var lastMonster = selectedAvailableMonsters.Where(m => m.SanitizedXp <= remainingBudget && !encounter.Contains(m))
                                              .OrderByDescending(m => m.SanitizedXp).FirstOrDefault();
                    if (lastMonster != null)
                    {
                        encounter.Add(lastMonster);
                        remainingBudget -= lastMonster.SanitizedXp;
                    }
                }

                //Make sure to only include one monster with partyLevel+1 in the encounter
                if(IsCrPlusOneAlreadyIncludedInEncounter(encounter, partyLevel))
                    selectedAvailableMonsters = selectedAvailableMonsters.Where(m => m.SanitizedCr == partyLevel).ToList();
            }

            return encounter;
        }

        private static bool IsCrPlusOneAlreadyIncludedInEncounter(IList<Dnd5eMonsterDto> encounter, int partyLevel)
            => encounter.Any(m => m.SanitizedCr == partyLevel + 1);
    }

    /// <summary>
    /// Formats Dnd5eMonsterDto data into readable text format
    /// </summary>
    public static class MonsterDataFormatter
    {
        /// <summary>
        /// Formats a single monster's data into readable text
        /// </summary>
        /// <param name="monster">The monster to format</param>
        /// <returns>Formatted text representation of the monster</returns>
        public static string FormatMonsterData(Dnd5eMonsterDto monster)
        {
            var sb = new System.Text.StringBuilder();
            
            // Basic Information
            sb.AppendLine($"Name: {monster.Name}");
            sb.AppendLine($"Size: {monster.Size}");
            sb.AppendLine($"Type: {monster.Type}");
            sb.AppendLine($"Alignment: {monster.Alignment}");
            sb.AppendLine($"Habitat: {monster.Habitat}");
            sb.AppendLine($"Main Habitat: {monster.MainHabitat}");
            sb.AppendLine($"Other Habitat: {monster.OtherHabitat}");
            sb.AppendLine($"Treasure: {monster.Treasure}");
            sb.AppendLine();

            // Combat Stats
            sb.AppendLine($"Armor Class: {monster.AC}");
            sb.AppendLine($"Hit Points: {monster.HP}");
            sb.AppendLine($"Initiative: {monster.Initiative}");
            sb.AppendLine();

            // Movement
            sb.AppendLine($"Walk: {monster.Walk}");
            sb.AppendLine($"Burrow: {monster.Burrow}");
            sb.AppendLine($"Climb: {monster.Climb}");
            sb.AppendLine($"Fly: {monster.Fly}");
            sb.AppendLine($"Hover: {monster.Hover}");
            sb.AppendLine($"Swim: {monster.Swim}");
            sb.AppendLine();

            // Ability Scores
            sb.AppendLine($"Strength: {monster.STR}");
            sb.AppendLine($"Dexterity: {monster.DEX}");
            sb.AppendLine($"Constitution: {monster.CON}");
            sb.AppendLine($"Intelligence: {monster.INT}");
            sb.AppendLine($"Wisdom: {monster.WIS}");
            sb.AppendLine($"Charisma: {monster.CHA}");
            sb.AppendLine();

            // Saving Throws
            if (!string.IsNullOrEmpty(monster.STR_Save))
                sb.AppendLine($"Strength Save: {monster.STR_Save}");
            if (!string.IsNullOrEmpty(monster.DEX_Save))
                sb.AppendLine($"Dexterity Save: {monster.DEX_Save}");
            if (!string.IsNullOrEmpty(monster.CON_Save))
                sb.AppendLine($"Constitution Save: {monster.CON_Save}");
            if (!string.IsNullOrEmpty(monster.INT_Save))
                sb.AppendLine($"Intelligence Save: {monster.INT_Save}");
            if (!string.IsNullOrEmpty(monster.WIS_Save))
                sb.AppendLine($"Wisdom Save: {monster.WIS_Save}");
            if (!string.IsNullOrEmpty(monster.CHA_Save))
                sb.AppendLine($"Charisma Save: {monster.CHA_Save}");
            sb.AppendLine();

            // Skills and Proficiencies
            if (!string.IsNullOrEmpty(monster.Proficient))
                sb.AppendLine($"Proficient Skills: {monster.Proficient}");
            if (!string.IsNullOrEmpty(monster.Expertise))
                sb.AppendLine($"Expertise: {monster.Expertise}");
            sb.AppendLine();

            // Damage Vulnerabilities and Immunities
            if (!string.IsNullOrEmpty(monster.Vulnerabilities))
                sb.AppendLine($"Vulnerabilities: {monster.Vulnerabilities}");
            if (!string.IsNullOrEmpty(monster.Slashing))
                sb.AppendLine($"Slashing: {monster.Slashing}");
            if (!string.IsNullOrEmpty(monster.ImmunitiesConditions))
                sb.AppendLine($"Condition Immunities: {monster.ImmunitiesConditions}");
            if (!string.IsNullOrEmpty(monster.ImmunitiesDamage))
                sb.AppendLine($"Damage Immunities: {monster.ImmunitiesDamage}");
            sb.AppendLine();

            // Senses
            if (!string.IsNullOrEmpty(monster.Blindsight))
                sb.AppendLine($"Blindsight: {monster.Blindsight}");
            if (!string.IsNullOrEmpty(monster.Darkvision))
                sb.AppendLine($"Darkvision: {monster.Darkvision}");
            if (!string.IsNullOrEmpty(monster.Truesight))
                sb.AppendLine($"Truesight: {monster.Truesight}");
            if (!string.IsNullOrEmpty(monster.Tremorsense))
                sb.AppendLine($"Tremorsense: {monster.Tremorsense}");
            if (!string.IsNullOrEmpty(monster.PassivePerception))
                sb.AppendLine($"Passive Perception: {monster.PassivePerception}");
            sb.AppendLine();

            // Languages and Challenge
            sb.AppendLine($"Languages: {monster.Languages}");
            sb.AppendLine($"Challenge Rating: {monster.CR}");
            sb.AppendLine($"Experience Points: {monster.XP}");
            sb.AppendLine($"Proficiency Bonus: {monster.PB}");
            sb.AppendLine();

            // Traits and Special Abilities
            if (!string.IsNullOrEmpty(monster.Traits))
                sb.AppendLine($"Traits: {monster.Traits}");
            if (!string.IsNullOrEmpty(monster.NumberOfLegendaryResistance))
                sb.AppendLine($"Legendary Resistances: {monster.NumberOfLegendaryResistance}");
            sb.AppendLine();

            // Attacks
            if (!string.IsNullOrEmpty(monster.NumberOfAtk))
                sb.AppendLine($"Number of Attacks: {monster.NumberOfAtk}");
            
            // Attack 1
            if (!string.IsNullOrEmpty(monster.Atk1Type))
            {
                sb.AppendLine($"Attack 1 - Type: {monster.Atk1Type}");
                sb.AppendLine($"Attack 1 - Modifier: {monster.Atk1Mod}");
                sb.AppendLine($"Attack 1 - Range: {monster.Atk1Range}");
                sb.AppendLine($"Attack 1 - Short Range: {monster.Atk1RangeShort}");
                sb.AppendLine($"Attack 1 - Damage: {monster.Atk1Damage}");
                sb.AppendLine($"Attack 1 - Damage Type: {monster.Atk1DamageType}");
                sb.AppendLine();
            }

            // Attack 2
            if (!string.IsNullOrEmpty(monster.Atk2Type))
            {
                sb.AppendLine($"Attack 2 - Type: {monster.Atk2Type}");
                sb.AppendLine($"Attack 2 - Modifier: {monster.Atk2Mod}");
                sb.AppendLine($"Attack 2 - Range: {monster.Atk2Range}");
                sb.AppendLine($"Attack 2 - Short Range: {monster.Atk2RangeShort}");
                sb.AppendLine($"Attack 2 - Damage: {monster.Atk2Damage}");
                sb.AppendLine($"Attack 2 - Damage Type: {monster.Atk2DamageType}");
                sb.AppendLine();
            }

            // Attack 3
            if (!string.IsNullOrEmpty(monster.Atk3Type))
            {
                sb.AppendLine($"Attack 3 - Type: {monster.Atk3Type}");
                sb.AppendLine($"Attack 3 - Modifier: {monster.Atk3Mod}");
                sb.AppendLine($"Attack 3 - Range: {monster.Atk3Range}");
                sb.AppendLine($"Attack 3 - Short Range: {monster.Atk3RangeShort}");
                sb.AppendLine($"Attack 3 - Damage: {monster.Atk3Damage}");
                sb.AppendLine($"Attack 3 - Damage Type: {monster.Atk3DamageType}");
                sb.AppendLine();
            }

            // Attack 4
            if (!string.IsNullOrEmpty(monster.Atk4Type))
            {
                sb.AppendLine($"Attack 4 - Type: {monster.Atk4Type}");
                sb.AppendLine($"Attack 4 - Modifier: {monster.Atk4Mod}");
                sb.AppendLine($"Attack 4 - Range: {monster.Atk4Range}");
                sb.AppendLine($"Attack 4 - Short Range: {monster.Atk4RangeShort}");
                sb.AppendLine($"Attack 4 - Damage: {monster.Atk4Damage}");
                sb.AppendLine($"Attack 4 - Damage Type: {monster.Atk4DamageType}");
                sb.AppendLine();
            }

            // Save DC and Actions
            if (!string.IsNullOrEmpty(monster.SaveDC))
                sb.AppendLine($"Save DC: {monster.SaveDC}");
            if (!string.IsNullOrEmpty(monster.SavingThrow))
                sb.AppendLine($"Saving Throw: {monster.SavingThrow}");
            if (!string.IsNullOrEmpty(monster.ActionNotes))
                sb.AppendLine($"Action Notes: {monster.ActionNotes}");
            if (!string.IsNullOrEmpty(monster.Ability))
                sb.AppendLine($"Ability: {monster.Ability}");
            sb.AppendLine();

            // Spellcasting
            if (!string.IsNullOrEmpty(monster.SpellSaveDC))
                sb.AppendLine($"Spell Save DC: {monster.SpellSaveDC}");
            if (!string.IsNullOrEmpty(monster.SpellSavingThrows))
                sb.AppendLine($"Spell Saving Throws: {monster.SpellSavingThrows}");
            if (!string.IsNullOrEmpty(monster.SpellAttack))
                sb.AppendLine($"Spell Attack: {monster.SpellAttack}");
            sb.AppendLine();

            // Spell Usage
            if (!string.IsNullOrEmpty(monster.AtWill))
                sb.AppendLine($"At Will: {monster.AtWill}");
            if (!string.IsNullOrEmpty(monster.ThreePerDay))
                sb.AppendLine($"3/Day: {monster.ThreePerDay}");
            if (!string.IsNullOrEmpty(monster.TwoPerDay))
                sb.AppendLine($"2/Day: {monster.TwoPerDay}");
            if (!string.IsNullOrEmpty(monster.OnePerDay))
                sb.AppendLine($"1/Day: {monster.OnePerDay}");
            sb.AppendLine();

            // Bonus Actions and Reactions
            if (!string.IsNullOrEmpty(monster.BonusAction))
                sb.AppendLine($"Bonus Action: {monster.BonusAction}");
            if (!string.IsNullOrEmpty(monster.Reaction))
                sb.AppendLine($"Reaction: {monster.Reaction}");
            if (!string.IsNullOrEmpty(monster.Amount))
                sb.AppendLine($"Amount: {monster.Amount}");
            sb.AppendLine();

            // Legendary Actions
            if (!string.IsNullOrEmpty(monster.LegendaryActionSaveDC))
                sb.AppendLine($"Legendary Action Save DC: {monster.LegendaryActionSaveDC}");
            if (!string.IsNullOrEmpty(monster.LegendaryActionSavingThrow))
                sb.AppendLine($"Legendary Action Saving Throw: {monster.LegendaryActionSavingThrow}");
            if (!string.IsNullOrEmpty(monster.LegendaryActions))
                sb.AppendLine($"Legendary Actions: {monster.LegendaryActions}");
            sb.AppendLine();

            // Lair Actions
            if (!string.IsNullOrEmpty(monster.Lair))
                sb.AppendLine($"Lair: {monster.Lair}");
            if (!string.IsNullOrEmpty(monster.LegendaryResistance))
                sb.AppendLine($"Legendary Resistance: {monster.LegendaryResistance}");
            if (!string.IsNullOrEmpty(monster.LairSaveDC))
                sb.AppendLine($"Lair Save DC: {monster.LairSaveDC}");
            if (!string.IsNullOrEmpty(monster.LairSavingThrows))
                sb.AppendLine($"Lair Saving Throws: {monster.LairSavingThrows}");
            sb.AppendLine();

            // Other
            if (!string.IsNullOrEmpty(monster.Other))
                sb.AppendLine($"Other: {monster.Other}");

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Formats a list of monsters into readable text
        /// </summary>
        /// <param name="monsters">List of monsters to format</param>
        /// <returns>Formatted text representation of all monsters</returns>
        public static string FormatMonstersList(IEnumerable<Dnd5eMonsterDto> monsters)
        {
            var sb = new System.Text.StringBuilder();
            var monsterList = monsters.ToList();
            
            for (int i = 0; i < monsterList.Count; i++)
            {
                sb.AppendLine($"=== MONSTER {i + 1} ===");
                sb.AppendLine(FormatMonsterData(monsterList[i]));
                sb.AppendLine();
                
                if (i < monsterList.Count - 1)
                {
                    sb.AppendLine("---");
                    sb.AppendLine();
                }
            }
            
            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Formats a list of monsters into a compact summary format
        /// </summary>
        /// <param name="monsters">List of monsters to format</param>
        /// <returns>Compact summary of all monsters</returns>
        public static string FormatMonstersSummary(IEnumerable<Dnd5eMonsterDto> monsters)
        {
            var sb = new System.Text.StringBuilder();
            var monsterList = monsters.ToList();
            
            sb.AppendLine($"Encounter Summary - {monsterList.Count} Monsters:");
            sb.AppendLine();
            
            for (int i = 0; i < monsterList.Count; i++)
            {
                var monster = monsterList[i];
                sb.AppendLine($"{i + 1}. {monster.Name}");
                sb.AppendLine($"   CR: {monster.CR} | XP: {monster.XP} | HP: {monster.HP} | AC: {monster.AC}");
                sb.AppendLine($"   Type: {monster.Type} | Size: {monster.Size}");
                sb.AppendLine($"   STR: {monster.STR} | DEX: {monster.DEX} | CON: {monster.CON} | INT: {monster.INT} | WIS: {monster.WIS} | CHA: {monster.CHA}");
                sb.AppendLine();
            }
            
            return sb.ToString().TrimEnd();
        }
    }
}
