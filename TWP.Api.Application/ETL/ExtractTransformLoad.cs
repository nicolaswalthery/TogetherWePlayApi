using Common.ResultPattern;
using System.Text.Json;
using TWP.Api.Application.ETL.Services;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.Interops;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.ETL
{
    public class ExtractTransformLoad : IExtractTransformLoad
    {
        private readonly string _folderPath = "C:\\Users\\nicol\\source\\repos\\nicolaswalthery\\TogetherWePlayApi\\TWP.Api.Infrastructure\\DndMonsterImages";
        private readonly IAideDdMonster5eRepository _aideDdMonster5ERepository;
        private readonly IAideDdInterops _aideDdInterops;
        private readonly IActionMapperService _actionMapperService;
        private readonly ITraitMapperService _traitMapperService;
        private readonly IMonster5eRepository _monster5eRepository;
        private readonly IOpenAiInterops _openAiInterops;

        public ExtractTransformLoad(
            IAideDdMonster5eRepository aideDdMonster5ERepository,
            IAideDdInterops aideDdInterops,
            IActionMapperService actionMapperService,
            ITraitMapperService traitMapperService,
            IMonster5eRepository monster5eRepository,
            IOpenAiInterops openAiInterops)
        {
            _aideDdMonster5ERepository = aideDdMonster5ERepository;
            _aideDdInterops = aideDdInterops;
            _actionMapperService = actionMapperService;
            _traitMapperService = traitMapperService;
            _monster5eRepository = monster5eRepository;
            _openAiInterops = openAiInterops;
        }

        public async Task<Result> RunAideDdMonster5eEtl()
            => await Safe.ExecuteAsync(async () =>
            {
                var res = await _aideDdMonster5ERepository.FindByCrOrLessAsync(30);

                var monsterAlreadyLoaded = await _monster5eRepository.GetAllAsync();
                if(monsterAlreadyLoaded.IsFailure)
                    return Result.Failure(monsterAlreadyLoaded.Error, monsterAlreadyLoaded.ReasonType);

                var monsterDbEntities = new List<Monster5eDbEntity>();
                foreach (var aideDdMonsterMetadata in res.Data)
                {
                    if (monsterAlreadyLoaded.Data.Select(m => m.Name).Contains(aideDdMonsterMetadata.Name))
                        continue;

                    AideDdMonsterResponseDto aideDdMonster = new();

                    //Quick Fix : Related to AideDD data issues :/
                    //if (aideDdMonsterMetadata.Name == "will-o--wisp")
                    //    continue;//aideDdMonster = await _aideDdInterops.GetMonsterByName("will-o--wisp");

                    //if (aideDdMonsterMetadata.Name == "Yuan-ti Malison (Type 1)")
                    //    aideDdMonster = await _aideDdInterops.GetMonsterByName("yuan-ti-malison-type-1");

                    //if (aideDdMonsterMetadata.Name == "Yuan-ti Malison (Type 2)")
                    //    aideDdMonster = await _aideDdInterops.GetMonsterByName("yuan-ti-malison-type-2");

                    //if (aideDdMonsterMetadata.Name == "Yuan-ti Malison (Type 3)")
                    //    aideDdMonster = await _aideDdInterops.GetMonsterByName("yuan-ti-malison-type-3");

                    // Extract: Get monster data from AideDD
                    aideDdMonster = aideDdMonster is null ? await _aideDdInterops.GetMonsterByName(aideDdMonsterMetadata.Name) : aideDdMonster;

                    // Transform: Convert to DB entity
                    var monsterDbEntity = aideDdMonster.ToDbEntity();

                    // Transform: Map Actions using OpenAI
                    monsterDbEntity.Actions = await _actionMapperService.MapMonsterActionsAsync(
                        aideDdMonster,
                        monsterDbEntity.Id);

                    // Transform: Map Traits using OpenAI
                    monsterDbEntity.Traits = await _traitMapperService.MapMonsterTraitsAsync(
                        aideDdMonster,
                        monsterDbEntity.Id);

                    await _monster5eRepository.Insert(monsterDbEntity);
                }

                return Result.Success();
            });

        public async Task<Result> ImportMonstersFromImagesAsync()
            => await Safe.ExecuteAsync(async () =>
            {
                if (!Directory.Exists(_folderPath))
                    return Result.Failure($"Folder not found: {_folderPath}");

                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                var imageFiles = Directory.GetFiles(_folderPath)
                    .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .ToList();

                if (!imageFiles.Any())
                    return Result.Failure($"No image files found in {_folderPath}");

                var successCount = 0;
                var failedFiles = new List<string>();

                foreach (var imageFile in imageFiles)
                {
                    try
                    {
                        var result = await ImportMonsterFromImageAsync(imageFile);
                        if (result.IsSuccess)
                        {
                            successCount++;
                            Console.WriteLine($"✓ Successfully imported: {Path.GetFileName(imageFile)}");
                        }
                        else
                        {
                            failedFiles.Add(Path.GetFileName(imageFile));
                            Console.WriteLine($"✗ Failed to import: {Path.GetFileName(imageFile)} - {result.Error}");
                        }
                    }
                    catch (Exception ex)
                    {
                        failedFiles.Add(Path.GetFileName(imageFile));
                        Console.WriteLine($"✗ Error processing {Path.GetFileName(imageFile)}: {ex.Message}");
                    }

                    // Add delay to avoid rate limiting
                    await Task.Delay(1000);
                }

                var message = $"Import completed: {successCount}/{imageFiles.Count} monsters imported successfully.";
                if (failedFiles.Any())
                {
                    message += $" Failed files: {string.Join(", ", failedFiles)}";
                }

                return successCount > 0
                    ? Result.Success()
                    : Result.Failure(message);
            });

        private readonly string _extractionPrompt = @"
You are an expert at extracting D&D 5e monster stat blocks from images. 
Analyze this image and extract ALL information into a structured JSON format.

Return ONLY valid JSON with this exact structure:
{
  ""name"": ""Monster Name"",
  ""alignment"": ""alignment (e.g., 'ChaoticEvil', 'LawfulGood')"",
  ""size"": ""size (Tiny/Small/Medium/Large/Huge/Gargantuan)"",
  ""type"": ""creature type"",
  ""subtype"": ""creature subtype if any"",
  ""armorClass"": 15,
  ""hitPoints"": 100,
  ""hitDice"": ""12d10+36"",
  ""speed"": ""30 ft."",
  ""climb"": ""20 ft."",
  ""swim"": null,
  ""fly"": ""60 ft."",
  ""strength"": 18,
  ""dexterity"": 14,
  ""constitution"": 16,
  ""intelligence"": 10,
  ""wisdom"": 12,
  ""charisma"": 8,
  ""challengeRating"": ""5"",
  ""proficiencyBonus"": 3,
  ""savingThrows"": {
    ""str"": 7,
    ""con"": 6,
    ""wis"": 4
  },
  ""skills"": {
    ""Perception"": 4,
    ""Stealth"": 5
  },
  ""damageResistances"": ""fire, poison"",
  ""damageImmunities"": ""necrotic"",
  ""conditionImmunities"": ""charmed, frightened"",
  ""senses"": ""darkvision 60 ft., passive Perception 14"",
  ""languages"": ""Common, Draconic"",
  ""traits"": [
    {
      ""name"": ""Trait Name"",
      ""description"": ""Full trait description text""
    }
  ],
  ""actions"": [
    {
      ""name"": ""Action Name"",
      ""type"": ""Action"",
      ""attackType"": ""Melee"",
      ""description"": ""Full action description"",
      ""attackBonus"": 7,
      ""reach"": ""5 ft."",
      ""range"": null,
      ""damage"": ""2d6+4"",
      ""damageType"": ""slashing""
    }
  ],
  ""reactions"": [
    {
      ""name"": ""Reaction Name"",
      ""description"": ""Reaction description"",
      ""trigger"": ""When condition occurs""
    }
  ],
  ""legendaryActions"": [
    {
      ""name"": ""Legendary Action Name"",
      ""description"": ""Description"",
      ""cost"": 1
    }
  ],
  ""lairActions"": []
}

Extract ALL visible information. Use null for missing values. Be precise with numbers.";


        public async Task<Result> ImportMonsterFromImageAsync(string imagePath)
        {
            return await Safe.ExecuteAsync(async () =>
            {
                if (!File.Exists(imagePath))
                    return Result.Failure($"Image file not found: {imagePath}");

                // Convert image to base64
                var imageBase64 = Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath));

                // Call OpenAI Vision API
                var visionResponse = await _openAiInterops.AnalyzeImageAsync(imageBase64, _extractionPrompt);

                if (string.IsNullOrEmpty(visionResponse))
                    return Result.Failure("No response from OpenAI Vision API");

                // Parse the JSON response
                var monsterData = JsonSerializer.Deserialize<MonsterImageData>(
                    visionResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (monsterData == null)
                    return Result.Failure("Failed to parse monster data from image");

                // Convert to database entity
                var monsterEntity = ConvertToMonsterEntity(monsterData);

                // Check if monster already exists
                var existingMonster = await _monster5eRepository.GetByNameAsync(monsterEntity.Name);
                if (existingMonster.IsSuccess && existingMonster.Data != null)
                {
                    return Result.Failure($"Monster '{monsterEntity.Name}' already exists in database");
                }

                monsterEntity.Source = "UM5e";
                // Save to database
                await _monster5eRepository.Insert(monsterEntity);

                return Result.Success();
            });
        }

        private Monster5eDbEntity ConvertToMonsterEntity(MonsterImageData data)
        {
            var monster = new Monster5eDbEntity
            {
                Id = Guid.NewGuid(),
                Name = data.Name ?? "Unknown Monster",
                Alignment = ParseAlignment(data.Alignment),
                CreatureSize = ParseSize(data.Size),
                CreatureType = data.Type,
                CreatureSubType = data.Subtype,
                ArmorClass = data.ArmorClass ?? 10,
                HitPoints = data.HitPoints ?? 1,
                HitDice = data.HitDice ?? "",
                Speed = data.Speed,
                Climb = data.Climb,
                Swim = data.Swim,
                Fly = data.Fly,
                Strength = data.Strength,
                Dexterity = data.Dexterity,
                Constitution = data.Constitution,
                Intelligence = data.Intelligence,
                Wisdom = data.Wisdom,
                Charisma = data.Charisma,
                ChallengeRating = data.ChallengeRating ?? "0",
                Cr = ParseChallengeRating(data.ChallengeRating),
                ProficiencyBonus = data.ProficiencyBonus,
                DamageResistances = data.DamageResistances,
                DamageImmunities = data.DamageImmunities,
                Senses = data.Senses,
                Languages = data.Languages,
                Source = "Image Import",
                PageSource = 0
            };

            // Set saving throws
            if (data.SavingThrows != null)
            {
                monster.StrSavingThrow = data.SavingThrows.Str;
                monster.DexSavingThrow = data.SavingThrows.Dex;
                monster.ConSavingThrow = data.SavingThrows.Con;
                monster.IntSavingThrow = data.SavingThrows.Int;
                monster.WisSavingThrow = data.SavingThrows.Wis;
                monster.ChaSavingThrow = data.SavingThrows.Cha;
            }

            // Convert skills to JSON
            if (data.Skills != null && data.Skills.Any())
            {
                monster.Skills = JsonSerializer.Serialize(data.Skills);
            }

            // Calculate XP from CR
            monster.Xp = CalculateXpFromCr(monster.Cr);

            // Add traits
            if (data.Traits != null)
            {
                foreach (var trait in data.Traits)
                {
                    monster.Traits.Add(new TraitDbEntity
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = monster.Id,
                        Title = trait.Name ?? "Unknown Trait",
                        Description = trait.Description ?? "",
                        IsOptional = false
                    });
                }
            }

            // Add actions
            if (data.Actions != null)
            {
                foreach (var action in data.Actions)
                {
                    monster.Actions.Add(ConvertToActionEntity(action, monster.Id));
                }
            }

            // Add reactions as actions with type Reaction
            if (data.Reactions != null)
            {
                foreach (var reaction in data.Reactions)
                {
                    monster.Actions.Add(new ActionDbEntity
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = monster.Id,
                        Name = reaction.Name ?? "Unknown Reaction",
                        Type = ActionTypeEnum.Reaction,
                        AttackType = AttackTypeEnum.None,
                        Description = reaction.Description ?? "",
                        actionTrigger = reaction.Trigger
                    });
                }
            }

            // Add legendary actions
            if (data.LegendaryActions != null)
            {
                foreach (var legendary in data.LegendaryActions)
                {
                    monster.Actions.Add(new ActionDbEntity
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = monster.Id,
                        Name = legendary.Name ?? "Unknown Legendary Action",
                        Type = ActionTypeEnum.Legendary,
                        AttackType = AttackTypeEnum.None,
                        Description = legendary.Description ?? "",
                        LimitPerDay = legendary.Cost
                    });
                }
            }

            return monster;
        }

        private ActionDbEntity ConvertToActionEntity(ActionImageData action, Guid monsterId)
        {
            var actionEntity = new ActionDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                Name = action.Name ?? "Unknown Action",
                Type = ParseActionType(action.Type),
                AttackType = ParseAttackType(action.AttackType),
                Description = action.Description ?? "",
                AttackBonus = action.AttackBonus,
                ShortRange = action.Range ?? action.Reach,
                IsProhibitedForMinion = false
            };

            // Parse damage if present
            if (!string.IsNullOrEmpty(action.Damage))
            {
                ParseDamage(action.Damage, actionEntity);
            }

            // Set damage type
            if (!string.IsNullOrEmpty(action.DamageType))
            {
                actionEntity.DamageType = ParseDamageType(action.DamageType);
            }

            return actionEntity;
        }

        private void ParseDamage(string damage, ActionDbEntity action)
        {
            // Parse damage strings like "2d6+4" or "1d8"
            var match = System.Text.RegularExpressions.Regex.Match(
                damage,
                @"(\d+)d(\d+)(?:\+(\d+))?");

            if (match.Success)
            {
                action.NumberDamageDice = int.Parse(match.Groups[1].Value);
                var diceSize = int.Parse(match.Groups[2].Value);
                action.DamageDice = ParseDiceType(diceSize);

                if (match.Groups[3].Success)
                {
                    action.DamageBonus = int.Parse(match.Groups[3].Value);
                }
            }
        }

        private float ParseChallengeRating(string? cr)
        {
            if (string.IsNullOrEmpty(cr)) return 0;

            // Handle fractions like "1/4", "1/2", "1/8"
            if (cr.Contains("/"))
            {
                var parts = cr.Split('/');
                if (parts.Length == 2 &&
                    float.TryParse(parts[0], out var numerator) &&
                    float.TryParse(parts[1], out var denominator))
                {
                    return numerator / denominator;
                }
            }

            return float.TryParse(cr, out var result) ? result : 0;
        }

        private int CalculateXpFromCr(float cr)
        {
            // Standard D&D 5e XP values by CR
            var xpByCr = new Dictionary<float, int>
            {
                { 0, 10 }, { 0.125f, 25 }, { 0.25f, 50 }, { 0.5f, 100 },
                { 1, 200 }, { 2, 450 }, { 3, 700 }, { 4, 1100 },
                { 5, 1800 }, { 6, 2300 }, { 7, 2900 }, { 8, 3900 },
                { 9, 5000 }, { 10, 5900 }, { 11, 7200 }, { 12, 8400 },
                { 13, 10000 }, { 14, 11500 }, { 15, 13000 }, { 16, 15000 },
                { 17, 18000 }, { 18, 20000 }, { 19, 22000 }, { 20, 25000 },
                { 21, 33000 }, { 22, 41000 }, { 23, 50000 }, { 24, 62000 },
                { 25, 75000 }, { 26, 90000 }, { 27, 105000 }, { 28, 120000 },
                { 29, 135000 }, { 30, 155000 }
            };

            return xpByCr.ContainsKey(cr) ? xpByCr[cr] : 0;
        }

        private AlignmentEnum? ParseAlignment(string? alignment)
        {
            if (string.IsNullOrEmpty(alignment)) return null;

            var normalizedAlignment = alignment.Replace(" ", "").ToLower();

            return normalizedAlignment switch
            {
                "lawfulgood" or "lg" => AlignmentEnum.LawfulGood,
                "neutralgood" or "ng" => AlignmentEnum.NeutralGood,
                "chaoticgood" or "cg" => AlignmentEnum.ChaoticGood,
                "lawfulneutral" or "ln" => AlignmentEnum.LawfulNeutral,
                "trueneutral" or "tn" or "neutral" or "n" => AlignmentEnum.TrueNeutral,
                "chaoticneutral" or "cn" => AlignmentEnum.ChaoticNeutral,
                "lawfulevil" or "le" => AlignmentEnum.LawfulEvil,
                "neutralevil" or "ne" => AlignmentEnum.NeutralEvil,
                "chaoticevil" or "ce" => AlignmentEnum.ChaoticEvil,
                "unaligned" => AlignmentEnum.Unaligned,
                "any" => AlignmentEnum.Any,
                _ => AlignmentEnum.Unaligned
            };
        }

        private CreatureSizeEnum ParseSize(string? size)
        {
            if (string.IsNullOrEmpty(size)) return CreatureSizeEnum.Medium;

            return size.ToLower() switch
            {
                "tiny" => CreatureSizeEnum.Tiny,
                "small" => CreatureSizeEnum.Small,
                "medium" => CreatureSizeEnum.Medium,
                "large" => CreatureSizeEnum.Large,
                "huge" => CreatureSizeEnum.Huge,
                "gargantuan" => CreatureSizeEnum.Gargantuan,
                _ => CreatureSizeEnum.Medium
            };
        }

        private ActionTypeEnum ParseActionType(string? type)
        {
            if (string.IsNullOrEmpty(type)) return ActionTypeEnum.Action;

            return type.ToLower() switch
            {
                "action" => ActionTypeEnum.Action,
                "bonus" or "bonusaction" => ActionTypeEnum.Bonus,
                "reaction" => ActionTypeEnum.Reaction,
                "legendary" => ActionTypeEnum.Legendary,
                "lair" => ActionTypeEnum.Lair,
                "movement" => ActionTypeEnum.Movement,
                _ => ActionTypeEnum.Action
            };
        }

        private AttackTypeEnum ParseAttackType(string? type)
        {
            if (string.IsNullOrEmpty(type)) return AttackTypeEnum.None;

            return type.ToLower() switch
            {
                "melee" => AttackTypeEnum.Melee,
                "ranged" => AttackTypeEnum.Ranged,
                "both" or "meleeorranged" => AttackTypeEnum.MeleeOrRanged,
                _ => AttackTypeEnum.None
            };
        }

        private DamageTypeEnum? ParseDamageType(string type)
        {
            return type.ToLower() switch
            {
                "acid" => DamageTypeEnum.Acid,
                "bludgeoning" => DamageTypeEnum.Bludgeoning,
                "cold" => DamageTypeEnum.Cold,
                "fire" => DamageTypeEnum.Fire,
                "force" => DamageTypeEnum.Force,
                "lightning" => DamageTypeEnum.Lightning,
                "necrotic" => DamageTypeEnum.Necrotic,
                "piercing" => DamageTypeEnum.Piercing,
                "poison" => DamageTypeEnum.Poison,
                "psychic" => DamageTypeEnum.Psychic,
                "radiant" => DamageTypeEnum.Radiant,
                "slashing" => DamageTypeEnum.Slashing,
                "thunder" => DamageTypeEnum.Thunder,
                _ => null
            };
        }

        private DiceTypeEnum? ParseDiceType(int size)
        {
            return size switch
            {
                4 => DiceTypeEnum.d4,
                6 => DiceTypeEnum.d6,
                8 => DiceTypeEnum.d8,
                10 => DiceTypeEnum.d10,
                12 => DiceTypeEnum.d12,
                20 => DiceTypeEnum.d20,
                100 => DiceTypeEnum.d100,
                _ => null
            };
        }
    }

    // DTOs for JSON parsing
    public class MonsterImageData
    {
        public string? Name { get; set; }
        public string? Alignment { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }
        public string? Subtype { get; set; }
        public int? ArmorClass { get; set; }
        public int? HitPoints { get; set; }
        public string? HitDice { get; set; }
        public string? Speed { get; set; }
        public string? Climb { get; set; }
        public string? Swim { get; set; }
        public string? Fly { get; set; }
        public int? Strength { get; set; }
        public int? Dexterity { get; set; }
        public int? Constitution { get; set; }
        public int? Intelligence { get; set; }
        public int? Wisdom { get; set; }
        public int? Charisma { get; set; }
        public string? ChallengeRating { get; set; }
        public int? ProficiencyBonus { get; set; }
        public SavingThrowsData? SavingThrows { get; set; }
        public Dictionary<string, int>? Skills { get; set; }
        public string? DamageResistances { get; set; }
        public string? DamageImmunities { get; set; }
        public string? ConditionImmunities { get; set; }
        public string? Senses { get; set; }
        public string? Languages { get; set; }
        public List<TraitImageData>? Traits { get; set; }
        public List<ActionImageData>? Actions { get; set; }
        public List<ReactionImageData>? Reactions { get; set; }
        public List<LegendaryActionImageData>? LegendaryActions { get; set; }
        public List<LairActionImageData>? LairActions { get; set; }
    }

    public class SavingThrowsData
    {
        public int? Str { get; set; }
        public int? Dex { get; set; }
        public int? Con { get; set; }
        public int? Int { get; set; }
        public int? Wis { get; set; }
        public int? Cha { get; set; }
    }

    public class TraitImageData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class ActionImageData
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? AttackType { get; set; }
        public string? Description { get; set; }
        public int? AttackBonus { get; set; }
        public string? Reach { get; set; }
        public string? Range { get; set; }
        public string? Damage { get; set; }
        public string? DamageType { get; set; }
    }

    public class ReactionImageData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Trigger { get; set; }
    }

    public class LegendaryActionImageData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Cost { get; set; }
    }

    public class LairActionImageData
    {
        public string? Description { get; set; }
    }
}
