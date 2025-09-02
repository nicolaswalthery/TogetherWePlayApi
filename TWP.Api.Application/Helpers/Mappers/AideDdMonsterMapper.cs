using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Helpers.Mappers { 

public static class AideDdMonsterMapper
{
    public static Monster5eDbEntity ToDbEntity(this AideDdMonsterResponseDto dto, string? defaultMonsterGroup = null, string? defaultShadow = null)
    {
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        // Parse identity bits from dto.TypeAndSubtype → Size, Type, Subtype, Alignment
        var (size, ctype, subtype, alignment) = ParseTypeAndAlignment(dto.TypeAndSubtype, dto.Alignment);

        // Parse AC / HP / HitDice
        var ac = ParseInt(dto.ArmorClass) ?? 0;
        var (hp, hitDice) = ParseHpAndDice(dto.HitPoints);

        // Parse Initiative (+x)
        var init = ParseFirstSignedInt(dto.Initiative) ?? ParseFirstSignedInt(dto.DexterityMod) ?? 0;

        // Parse speeds (walk + climb/swim/fly)
        var (speed, climb, swim, fly) = ParseSpeeds(dto.Speed);

        // Parse ability scores
        var str = ParseInt(dto.Strength);
        var dex = ParseInt(dto.Dexterity);
        var con = ParseInt(dto.Constitution);
        var @int = ParseInt(dto.Intelligence);
        var wis = ParseInt(dto.Wisdom);
        var cha = ParseInt(dto.Charisma);

        // Parse saving throws
        var strSave = ParseSigned(dto.StrengthSave);
        var dexSave = ParseSigned(dto.DexteritySave);
        var conSave = ParseSigned(dto.ConstitutionSave);
        var intSave = ParseSigned(dto.IntelligenceSave);
        var wisSave = ParseSigned(dto.WisdomSave);
        var chaSave = ParseSigned(dto.CharismaSave);

        // Compute XP from CR (fallback 0)
        var xp = ComputeXp(dto.ChallengeRating);

        // Serialize Skills (try to parse "Perception +9, Stealth +5" → JSON)
        var skillsJson = SerializeSkills(dto.Skills);

        // Serialize Gear → JSON
        var equipmentsJson = SerializeList(dto.Gear);

        // Build monster
        var monster = new Monster5eDbEntity
        {
            Name = dto.Name?.Trim() ?? string.Empty,
            Alignment = alignment,
            ChallengeRating = dto.ChallengeRating ?? "0",
            Xp = xp,
            InitiativeBonus = init,
            Role = null, // Optional: set if you infer roles
            CreatureSize = size,

            ArmorClass = ac,
            MinionArmorClass = null,
            HitPoints = hp,
            HitDice = hitDice,

            Speed = speed,
            Climb = climb,
            Swim = swim,
            Fly = fly,

            Strength = str,
            Dexterity = dex,
            Constitution = con,
            Intelligence = @int,
            Wisdom = wis,
            Charisma = cha,

            Skills = skillsJson,
            DamageImmunities = dto.Immunities?.Trim(),
            DamageResistances = dto.Resistances?.Trim(),
            Senses = dto.Senses?.Trim(),
            Languages = dto.Languages?.Trim(),

            ConSavingThrow = conSave,
            DexSavingThrow = dexSave,
            StrSavingThrow = strSave,
            WisSavingThrow = wisSave,
            ChaSavingThrow = chaSave,
            IntSavingThrow = intSave,
            ProficiencyBonus = null, // could be parsed from CR text if present

            Equipments = equipmentsJson,
            Habitats = dto.Habitat?.Trim(),
            CreatureType = ctype,
            CreatureSubType = subtype,
            MonsterGroup = defaultMonsterGroup ?? (subtype ?? ctype),

            Manner = null,
            Lore = null, // could store parsed lore as JSON if you add it to DTO
            PageSource = 0, // required column; set intelligently if you have it
            Source = dto.Source?.Trim() ?? string.Empty,
        };

        // ---- Traits ----
        if (dto.Traits?.Count > 0)
        {
            foreach (var raw in dto.Traits.Where(s => !string.IsNullOrWhiteSpace(s)))
                monster.Traits.Add(MapTrait(raw));
        }

        // ---- Optional one-to-one Symbarum ----
        if (!string.IsNullOrWhiteSpace(defaultShadow))
        {
            monster.Symbarum5e = new Symbarum5eDbEntity
            {
                Shadow = defaultShadow
            };
        }

        return monster;
    }

    // ----------------- Parsers & helpers -----------------

    private static (CreatureSizeEnum size, string? ctype, string? subtype, AlignmentEnum? align)
        ParseTypeAndAlignment(string? typeAndSubtype, string? fallbackAlignment)
    {
        // Example: "Huge Fiend (Demon), Chaotic Evil"
        var raw = (typeAndSubtype ?? "").Trim();
        var rx = new Regex(
            @"^(?<size>(?:Tiny|Small|Medium|Large|Huge|Gargantuan)(?:\s*(?:or|/)\s*(?:Tiny|Small|Medium|Large|Huge|Gargantuan))*)\s+" +
            @"(?<type>[A-Za-z][A-Za-z \-']*)(?:\s*\((?<subtype>[^)]+)\))?\s*,\s*(?<alignment>.+)$",
            RegexOptions.Compiled);

        var m = rx.Match(raw);
        var sizeStr = m.Success ? m.Groups["size"].Value : (raw.Split(' ').FirstOrDefault() ?? "Medium");
        var typeStr = m.Success ? m.Groups["type"].Value : null;
        var subStr = m.Success ? m.Groups["subtype"].Value : null;
        var alignStr = m.Success ? m.Groups["alignment"].Value : fallbackAlignment ?? "";

        return (ParseSize(sizeStr), typeStr?.Trim(), subStr?.Trim(), ParseAlignment(alignStr));
    }

    private static CreatureSizeEnum ParseSize(string s)
    {
        // Try clean parse, fallback to Medium
        var cleaned = (s ?? "Medium").Replace(" ", "", StringComparison.OrdinalIgnoreCase);
        return Enum.TryParse<CreatureSizeEnum>(cleaned, true, out var size) ? size : CreatureSizeEnum.Medium;
    }

    private static AlignmentEnum? ParseAlignment(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var t = s.Trim().ToLowerInvariant();

        return t switch
        {
            "any" or "any alignment" => AlignmentEnum.Any,
            "lawful good" => AlignmentEnum.LawfulGood,
            "neutral good" => AlignmentEnum.NeutralGood,
            "chaotic good" => AlignmentEnum.ChaoticGood,
            "lawful neutral" => AlignmentEnum.LawfulNeutral,
            "true neutral" or "neutral" => AlignmentEnum.TrueNeutral,
            "chaotic neutral" => AlignmentEnum.ChaoticNeutral,
            "lawful evil" => AlignmentEnum.LawfulEvil,
            "neutral evil" => AlignmentEnum.NeutralEvil,
            "chaotic evil" => AlignmentEnum.ChaoticEvil,
            "unaligned" => AlignmentEnum.Unaligned,
            _ => AlignmentEnum.Any
        };
    }

    private static (int hp, string hitDice) ParseHpAndDice(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return (0, "");
        // Examples:
        // "287 (23d12 + 138)"
        // "52 (8d8 +16)"
        var hp = ParseInt(s) ?? 0;
        var diceMatch = Regex.Match(s, @"\((?<dice>[^)]+)\)");
        var dice = diceMatch.Success ? diceMatch.Groups["dice"].Value.Replace(" ", "") : "";
        return (hp, dice);
    }

    private static (string? speed, string? climb, string? swim, string? fly) ParseSpeeds(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return (null, null, null, null);

        string? walk = null; string? climb = null; string? swim = null; string? fly = null;

        foreach (var part in s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var p = part.Trim();
            var move = Regex.Match(p, @"(?i)\b(?:(?<kind>walk|speed|climb|swim|fly|burrow))?\s*(?<val>\d+)\s*ft\.?");
            if (!move.Success) continue;
            var kind = (move.Groups["kind"].Success ? move.Groups["kind"].Value.ToLowerInvariant() : "speed");
            var val = $"{move.Groups["val"].Value} ft.";

            switch (kind)
            {
                case "climb": climb = val; break;
                case "swim": swim = val; break;
                case "fly": fly = val; break;
                case "speed":
                case "walk":
                default: if (walk == null) walk = val; break;
            }
        }

        // If nothing matched "walk/speed", set walk to first numeric found
        walk ??= Regex.Match(s, @"(\d+)\s*ft\.?", RegexOptions.IgnoreCase) is var m && m.Success ? $"{m.Groups[1].Value} ft." : null;

        return (walk, climb, swim, fly);
    }

    private static int? ParseInt(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var m = Regex.Match(s, @"-?\d+");
        return m.Success ? int.Parse(m.Value, CultureInfo.InvariantCulture) : (int?)null;
    }

    private static int? ParseFirstSignedInt(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var m = Regex.Match(s, @"[+\-]?\d+");
        return m.Success ? int.Parse(m.Value, CultureInfo.InvariantCulture) : (int?)null;
    }

    private static int? ParseSigned(string? s) => ParseFirstSignedInt(s);

    private static string? SerializeSkills(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;

        // Try parse formats like "Perception +9, Stealth +5"
        var dict = new Dictionary<string, int>();
        foreach (var part in s.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var m = Regex.Match(part.Trim(), @"^(?<name>[A-Za-z][A-Za-z \-']+)\s+(?<val>[+\-]?\d+)$");
            if (m.Success)
            {
                var key = m.Groups["name"].Value.Trim();
                var val = int.Parse(m.Groups["val"].Value, CultureInfo.InvariantCulture);
                dict[key] = val;
            }
        }

        if (dict.Count == 0) return s.Trim(); // store raw if not parsed
        return JsonSerializer.Serialize(dict);
    }

    private static string? SerializeList(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var list = s.Split(new[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToList();
        if (list.Count == 0) return null;
        return JsonSerializer.Serialize(list);
    }

    private static int ComputeXp(string? cr)
    {
        if (string.IsNullOrWhiteSpace(cr)) return 0;
        var table = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["0"] = 10,
            ["1/8"] = 25,
            ["1/4"] = 50,
            ["1/2"] = 100,
            ["1"] = 200,
            ["2"] = 450,
            ["3"] = 700,
            ["4"] = 1100,
            ["5"] = 1800,
            ["6"] = 2300,
            ["7"] = 2900,
            ["8"] = 3900,
            ["9"] = 5000,
            ["10"] = 5900,
            ["11"] = 7200,
            ["12"] = 8400,
            ["13"] = 10000,
            ["14"] = 11500,
            ["15"] = 13000,
            ["16"] = 15000,
            ["17"] = 18000,
            ["18"] = 20000,
            ["19"] = 22000,
            ["20"] = 25000,
            ["21"] = 33000,
            ["22"] = 41000,
            ["23"] = 50000,
            ["24"] = 62000,
            ["25"] = 75000,
            ["26"] = 90000,
            ["27"] = 105000,
            ["28"] = 120000,
            ["29"] = 135000,
            ["30"] = 155000
        };
        var key = cr.Trim().Substring(0, 2);
        return table.TryGetValue(key, out var xp) ? xp : (int.TryParse(key, out var n) && table.TryGetValue(n.ToString(), out var xp2) ? xp2 : 0);
    }

    // ---------- Action/Trait mapping ----------

    private static TraitDbEntity MapTrait(string raw)
    {
        var title = ExtractTitle(raw);
        return new TraitDbEntity
        {
            Title = title,
            Description = raw.Trim(),
            // Optionals default to null/false
            DamageBonus = null,
            AttackBonus = null,
            DamageDice = null,
            NumberDamageDice = null,
            DamageType = null,
            IsOptional = false,
            traitTrigger = ExtractLabeled(raw, "Trigger"),
            advantageCondition = ExtractLabeled(raw, "Advantage"),
            disadvantageCondition = ExtractLabeled(raw, "Disadvantage"),
        };
    }

    private static ActionDbEntity MapAction(string raw, ActionTypeEnum actionType)
    {
        var name = ExtractTitle(raw);
        var attackType = InferAttackType(raw);

        // Try parse bonuses, dice, type, ranges
        var attackBonus = ParseAttackBonus(raw);
        var (numDice, diceType, dmgBonus) = ParseDamageDice(raw);
        var dmgType = ParseDamageType(raw);
        var (shortRange, longRange) = ParseRanges(raw);

        return new ActionDbEntity
        {
            Name = name,
            Type = actionType,                            // REQUIRED by mapping :contentReference[oaicite:4]{index=4}
            AttackType = attackType,                      // REQUIRED by mapping :contentReference[oaicite:5]{index=5}
            Description = raw.Trim(),
            AttackBonus = attackBonus,
            NumberDamageDice = numDice,
            DamageDice = diceType,
            DamageBonus = dmgBonus,
            DamageType = dmgType,
            ShortRange = shortRange?.ToString(),
            LongRange = longRange?.ToString(),
            LimitPerDay = null,
            IsProhibitedForMinion = false,
            actionTrigger = ExtractLabeled(raw, "Trigger"),
            advantageCondition = ExtractLabeled(raw, "Advantage"),
            disadvantageCondition = ExtractLabeled(raw, "Disadvantage"),
        };
    }

    private static string ExtractTitle(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "Unnamed";
        // Cut at first '.' or ':' for a concise title
        var idx = raw.IndexOfAny(new[] { '.', ':' });
        return (idx > 0 ? raw[..idx] : raw).Trim().Trim('*', ' ', '\u200B');
    }

    private static AttackTypeEnum InferAttackType(string s)
    {
        var t = s.ToLowerInvariant();
        if (t.Contains("melee") && t.Contains("ranged")) return AttackTypeEnum.MeleeOrRanged;
        if (t.Contains("melee")) return AttackTypeEnum.Melee;
        if (t.Contains("ranged")) return AttackTypeEnum.Ranged;
        if (t.Contains("lair") || t.Contains("legendary") || t.Contains("recharge")) return AttackTypeEnum.Special;
        return AttackTypeEnum.NotSpecified;
    }

    private static int? ParseAttackBonus(string s)
    {
        // matches "... Attack Roll: +5" or "to hit +8"
        var m = Regex.Match(s, @"(?i)(?:attack roll|to hit)\s*:\s*([+\-]?\d+)|to hit\s*([+\-]?\d+)");
        if (m.Success)
        {
            for (int i = 1; i < m.Groups.Count; i++)
                if (m.Groups[i].Success) return int.Parse(m.Groups[i].Value, CultureInfo.InvariantCulture);
        }
        // fallback: first standalone signed number
        return ParseFirstSignedInt(s);
    }

    private static (int? numDice, DiceTypeEnum? diceType, int? dmgBonus) ParseDamageDice(string s)
    {
        // Examples: "Hit: 10 (2d6+3) Slashing", "(3d10 + 4) Fire"
        var m = Regex.Match(s, @"(?i)\((?<expr>\d+\s*d\s*\d+\s*(?:[+\-]\s*\d+)?)\)");
        if (!m.Success) return (null, null, null);

        var expr = m.Groups["expr"].Value.Replace(" ", "");
        var m2 = Regex.Match(expr, @"^(?<n>\d+)d(?<d>\d+)(?<bonus>[+\-]\d+)?$");
        if (!m2.Success) return (null, null, null);

        int? n = int.TryParse(m2.Groups["n"].Value, out var nn) ? nn : (int?)null;
        DiceTypeEnum? diceType = Enum.TryParse<DiceTypeEnum>("d" + m2.Groups["d"].Value, true, out DiceTypeEnum dt) ? dt : (DiceTypeEnum?)null;
        int? bonus = m2.Groups["bonus"].Success ? int.Parse(m2.Groups["bonus"].Value, CultureInfo.InvariantCulture) : (int?)null;
        return (n, diceType, bonus);
    }

    private static DamageTypeEnum? ParseDamageType(string s)
    {
        foreach (var name in Enum.GetNames(typeof(DamageTypeEnum)))
        {
            if (Regex.IsMatch(s, $@"(?i)\b{name}\b"))
                return Enum.Parse<DamageTypeEnum>(name);
        }
        return null;
    }

    private static (int? shortRange, int? longRange) ParseRanges(string s)
    {
        // "range 100/400 ft." → 100, 400
        var m = Regex.Match(s, @"(?i)\brange\s+(?<short>\d+)\s*/\s*(?<long>\d+)\s*ft");
        if (m.Success)
        {
            var sr = int.Parse(m.Groups["short"].Value, CultureInfo.InvariantCulture);
            var lr = int.Parse(m.Groups["long"].Value, CultureInfo.InvariantCulture);
            return (sr, lr);
        }
        return (null, null);
    }

    private static string? ExtractLabeled(string s, string label)
    {
        // "Trigger: foo. Response: bar." → "foo"
        var m = Regex.Match(s, $@"(?i)\b{Regex.Escape(label)}\s*:\s*(?<val>[^\.]+)");
        return m.Success ? m.Groups["val"].Value.Trim() : null;
    }
}
}

