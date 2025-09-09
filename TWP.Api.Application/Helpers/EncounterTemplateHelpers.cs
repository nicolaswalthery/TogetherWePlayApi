using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Helpers
{
    /// <summary>
    /// Static helper class providing D&D 5e encounter templates based on combat roles
    /// </summary>
    public static class EncounterTemplateHelpers
    {
        /// <summary>
        /// Represents a template for encounter composition
        /// </summary>
        public class EncounterTemplate
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public EncounterDifficultyEnum RecommendedDifficulty { get; set; }
            public List<RoleComposition> Composition { get; set; } = new();
            public int MinPartySize { get; set; } = 3;
            public int MaxPartySize { get; set; } = 6;
            public string TacticalNotes { get; set; }
        }

        /// <summary>
        /// Defines the composition of a role in an encounter
        /// </summary>
        public class RoleComposition
        {
            public CombatRoleEnum Role { get; set; }
            public int MinCount { get; set; }
            public int MaxCount { get; set; }
            public bool IsRequired { get; set; } = true;
            public string Notes { get; set; }

            /// <summary>
            /// Calculate actual count based on party size
            /// </summary>
            public int GetCountForPartySize(int partySize)
            {
                if (Role == CombatRoleEnum.Minion)
                {
                    // Minions scale with party size
                    return Math.Max(MinCount, partySize + (MaxCount - MinCount) / 2);
                }

                // Other roles use fixed or slightly scaled counts
                var scaleFactor = partySize / 4.0;
                var count = (int)Math.Round(MinCount * scaleFactor);
                return Math.Clamp(count, MinCount, MaxCount);
            }
        }

        /// <summary>
        /// Gets all available encounter templates
        /// </summary>
        public static List<EncounterTemplate> GetAllTemplates()
        {
            return new List<EncounterTemplate>
            {
                BossFight(),
                EliteSquad(),
                DefensiveLine(),
                AmbushParty(),
                SwarmTactics(),
                SiegeFormation(),
                HitAndRun(),
                TacticalCommand(),
                ChaosBrigade(),
                ControlPoint(),
                DungeonGuardians(),
                RaidingParty(),
                MageCircle(),
                AssassinStrike(),
                LastStand()
            };
        }

        /// <summary>
        /// Classic boss fight with minions
        /// </summary>
        public static EncounterTemplate BossFight() => new()
        {
            Name = "Boss Fight",
            Description = "A powerful solo enemy supported by expendable minions",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "The boss uses minions as shields while dealing massive damage. Focus fire recommended.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Solo, MinCount = 1, MaxCount = 1, Notes = "The main threat" },
                new() { Role = CombatRoleEnum.Minion, MinCount = 4, MaxCount = 8, Notes = "Expendable distractions" }
            }
        };

        /// <summary>
        /// Well-organized military unit
        /// </summary>
        public static EncounterTemplate EliteSquad() => new()
        {
            Name = "Elite Squad",
            Description = "A well-coordinated military unit with leadership and support",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "Take out the leader first to disrupt coordination, then focus on support.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Leader, MinCount = 1, MaxCount = 1, Notes = "Commands and buffs allies" },
                new() { Role = CombatRoleEnum.Soldier, MinCount = 2, MaxCount = 4, Notes = "Front line defenders" },
                new() { Role = CombatRoleEnum.Support, MinCount = 1, MaxCount = 2, Notes = "Heals and buffs" },
                new() { Role = CombatRoleEnum.Artillery, MinCount = 1, MaxCount = 2, IsRequired = false, Notes = "Optional ranged damage" }
            }
        };

        /// <summary>
        /// Defensive formation protecting a key target
        /// </summary>
        public static EncounterTemplate DefensiveLine() => new()
        {
            Name = "Defensive Line",
            Description = "A fortified position with layered defenses",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "Break through the front line to reach vulnerable artillery. Watch for support healing.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Soldier, MinCount = 3, MaxCount = 5, Notes = "Shield wall" },
                new() { Role = CombatRoleEnum.Artillery, MinCount = 2, MaxCount = 3, Notes = "Protected damage dealers" },
                new() { Role = CombatRoleEnum.Support, MinCount = 1, MaxCount = 1, Notes = "Keeps the line standing" }
            }
        };

        /// <summary>
        /// Surprise attack from multiple angles
        /// </summary>
        public static EncounterTemplate AmbushParty() => new()
        {
            Name = "Ambush Party",
            Description = "Fast strikers attacking from concealment",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "Expect attacks from multiple directions. Protect spellcasters and establish defensive positions quickly.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Ambusher, MinCount = 2, MaxCount = 4, Notes = "Initial strikers" },
                new() { Role = CombatRoleEnum.Skirmisher, MinCount = 2, MaxCount = 3, Notes = "Mobile harassers" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 0, MaxCount = 1, IsRequired = false, Notes = "Trap setter" }
            }
        };

        /// <summary>
        /// Overwhelming numbers of weak enemies
        /// </summary>
        public static EncounterTemplate SwarmTactics() => new()
        {
            Name = "Swarm Tactics",
            Description = "Overwhelming numbers controlled by a single mind",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "Use area effects. Identify and eliminate the controller to disrupt the swarm.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Minion, MinCount = 8, MaxCount = 16, Notes = "The swarm" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 1, MaxCount = 2, Notes = "Swarm coordinator" },
                new() { Role = CombatRoleEnum.Support, MinCount = 0, MaxCount = 1, IsRequired = false, Notes = "Minion buffer" }
            }
        };

        /// <summary>
        /// Heavy assault formation
        /// </summary>
        public static EncounterTemplate SiegeFormation() => new()
        {
            Name = "Siege Formation",
            Description = "Heavy hitters backed by long-range support",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "Kite the brutes while dealing with artillery. Controllers will try to keep you in kill zones.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Brute, MinCount = 2, MaxCount = 3, Notes = "Siege breakers" },
                new() { Role = CombatRoleEnum.Artillery, MinCount = 2, MaxCount = 4, Notes = "Bombardment" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 1, MaxCount = 2, Notes = "Battlefield control" }
            }
        };

        /// <summary>
        /// Mobile harassment force
        /// </summary>
        public static EncounterTemplate HitAndRun() => new()
        {
            Name = "Hit and Run",
            Description = "Mobile forces that strike and retreat",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "Pin them down with control effects. They'll try to kite and divide the party.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Skirmisher, MinCount = 3, MaxCount = 5, Notes = "Mobile strikers" },
                new() { Role = CombatRoleEnum.Ambusher, MinCount = 1, MaxCount = 2, Notes = "Opportunistic striker" },
                new() { Role = CombatRoleEnum.Artillery, MinCount = 0, MaxCount = 2, IsRequired = false, Notes = "Mobile fire support" }
            }
        };

        /// <summary>
        /// Organized tactical unit with strong leadership
        /// </summary>
        public static EncounterTemplate TacticalCommand() => new()
        {
            Name = "Tactical Command",
            Description = "Military precision with layered tactics",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "Disrupting the leader severely weakens the unit. Expect coordinated tactics and focus fire.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Leader, MinCount = 1, MaxCount = 1, Notes = "Tactical commander" },
                new() { Role = CombatRoleEnum.Soldier, MinCount = 2, MaxCount = 3, Notes = "Disciplined troops" },
                new() { Role = CombatRoleEnum.Artillery, MinCount = 1, MaxCount = 2, Notes = "Precision strikes" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 1, MaxCount = 1, Notes = "Battlefield tactician" }
            }
        };

        /// <summary>
        /// Disorganized but dangerous mob
        /// </summary>
        public static EncounterTemplate ChaosBrigade() => new()
        {
            Name = "Chaos Brigade",
            Description = "Unpredictable berserkers and opportunists",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "No coordination but high individual threat. Control and divide to conquer.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Brute, MinCount = 2, MaxCount = 4, Notes = "Rampaging force" },
                new() { Role = CombatRoleEnum.Skirmisher, MinCount = 2, MaxCount = 3, Notes = "Opportunists" },
                new() { Role = CombatRoleEnum.Minion, MinCount = 0, MaxCount = 6, IsRequired = false, Notes = "Cannon fodder" }
            }
        };

        /// <summary>
        /// Area denial and control specialists
        /// </summary>
        public static EncounterTemplate ControlPoint() => new()
        {
            Name = "Control Point",
            Description = "Defenders holding a strategic position",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "They control the battlefield. Break their formation or find alternative approaches.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Controller, MinCount = 2, MaxCount = 3, Notes = "Zone control" },
                new() { Role = CombatRoleEnum.Soldier, MinCount = 2, MaxCount = 4, Notes = "Point defenders" },
                new() { Role = CombatRoleEnum.Support, MinCount = 1, MaxCount = 2, Notes = "Sustain defense" },
                new() { Role = CombatRoleEnum.Artillery, MinCount = 0, MaxCount = 2, IsRequired = false, Notes = "Overwatch" }
            }
        };

        /// <summary>
        /// Dungeon defenders with varied threats
        /// </summary>
        public static EncounterTemplate DungeonGuardians() => new()
        {
            Name = "Dungeon Guardians",
            Description = "Mixed defenders adapted to their environment",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "Expect environmental hazards. Guardians know the terrain and will use it.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Soldier, MinCount = 1, MaxCount = 2, Notes = "Stalwart guards" },
                new() { Role = CombatRoleEnum.Brute, MinCount = 1, MaxCount = 2, Notes = "Heavy defender" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 1, MaxCount = 1, Notes = "Trap master" },
                new() { Role = CombatRoleEnum.Minion, MinCount = 2, MaxCount = 6, IsRequired = false, Notes = "Dungeon spawn" }
            }
        };

        /// <summary>
        /// Fast-moving raid force
        /// </summary>
        public static EncounterTemplate RaidingParty() => new()
        {
            Name = "Raiding Party",
            Description = "Fast strikers looking for quick victory",
            RecommendedDifficulty = EncounterDifficultyEnum.Moderate,
            TacticalNotes = "They'll try to overwhelm quickly. Survive the initial assault and counter-attack.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Skirmisher, MinCount = 2, MaxCount = 4, Notes = "Raiders" },
                new() { Role = CombatRoleEnum.Brute, MinCount = 1, MaxCount = 2, Notes = "Shock troops" },
                new() { Role = CombatRoleEnum.Leader, MinCount = 0, MaxCount = 1, IsRequired = false, Notes = "Raid leader" }
            }
        };

        /// <summary>
        /// Spellcaster-focused encounter
        /// </summary>
        public static EncounterTemplate MageCircle() => new()
        {
            Name = "Mage Circle",
            Description = "Spellcasters with protective escorts",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "Disrupt concentration and close distance quickly. Watch for area control spells.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Artillery, MinCount = 2, MaxCount = 3, Notes = "Spellcasters" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 1, MaxCount = 2, Notes = "Battlefield manipulator" },
                new() { Role = CombatRoleEnum.Soldier, MinCount = 1, MaxCount = 3, Notes = "Bodyguards" },
                new() { Role = CombatRoleEnum.Support, MinCount = 0, MaxCount = 1, IsRequired = false, Notes = "Enchanter" }
            }
        };

        /// <summary>
        /// Precision elimination squad
        /// </summary>
        public static EncounterTemplate AssassinStrike() => new()
        {
            Name = "Assassin Strike",
            Description = "Precision strikers targeting key party members",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "They'll focus your weakest defenses. Protect vulnerable allies and watch the shadows.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Ambusher, MinCount = 2, MaxCount = 3, Notes = "Assassins" },
                new() { Role = CombatRoleEnum.Controller, MinCount = 1, MaxCount = 1, Notes = "Setup specialist" },
                new() { Role = CombatRoleEnum.Skirmisher, MinCount = 0, MaxCount = 2, IsRequired = false, Notes = "Pursuit specialist" }
            }
        };

        /// <summary>
        /// Desperate defenders making a final stand
        /// </summary>
        public static EncounterTemplate LastStand() => new()
        {
            Name = "Last Stand",
            Description = "Desperate defenders fighting to the death",
            RecommendedDifficulty = EncounterDifficultyEnum.High,
            TacticalNotes = "They won't retreat and fight more dangerously as they weaken. No quarter given.",
            Composition = new List<RoleComposition>
            {
                new() { Role = CombatRoleEnum.Soldier, MinCount = 2, MaxCount = 3, Notes = "Determined defenders" },
                new() { Role = CombatRoleEnum.Leader, MinCount = 1, MaxCount = 1, Notes = "Inspiring presence" },
                new() { Role = CombatRoleEnum.Support, MinCount = 1, MaxCount = 2, Notes = "Keeping hope alive" },
                new() { Role = CombatRoleEnum.Brute, MinCount = 0, MaxCount = 2, IsRequired = false, Notes = "Berserkers" }
            }
        };

        /// <summary>
        /// Get a random template suitable for the given difficulty
        /// </summary>
        public static EncounterTemplate GetRandomTemplate(EncounterDifficultyEnum difficulty)
        {
            var templates = GetAllTemplates()
                .Where(t => t.RecommendedDifficulty == difficulty ||
                           t.RecommendedDifficulty == EncounterDifficultyEnum.Moderate)
                .ToList();

            if (!templates.Any())
                templates = GetAllTemplates();

            var random = new Random();
            return templates[random.Next(templates.Count)];
        }

        /// <summary>
        /// Get templates suitable for a specific party size
        /// </summary>
        public static List<EncounterTemplate> GetTemplatesForPartySize(int partySize)
        {
            return GetAllTemplates()
                .Where(t => partySize >= t.MinPartySize && partySize <= t.MaxPartySize)
                .ToList();
        }

        /// <summary>
        /// Calculate the expected CR budget multiplier for a template
        /// </summary>
        public static float GetCRBudgetMultiplier(EncounterTemplate template)
        {
            // Templates with more synergistic roles should have a higher multiplier
            var hasLeader = template.Composition.Any(c => c.Role == CombatRoleEnum.Leader);
            var hasSupport = template.Composition.Any(c => c.Role == CombatRoleEnum.Support);
            var hasController = template.Composition.Any(c => c.Role == CombatRoleEnum.Controller);

            float multiplier = 1.0f;

            if (hasLeader) multiplier += 0.1f;
            if (hasSupport) multiplier += 0.1f;
            if (hasController) multiplier += 0.15f;

            // Synergistic combinations
            if (hasLeader && hasSupport) multiplier += 0.1f;
            if (hasController && template.Composition.Any(c => c.Role == CombatRoleEnum.Artillery)) multiplier += 0.1f;

            return multiplier;
        }
    }
}