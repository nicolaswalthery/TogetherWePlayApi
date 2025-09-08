using Common.Randomizer;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Helpers
{
    //TODO : Should be a BusinessLayer with ChallengeRatingTable replaced by a MonsterBuildingGuidelineRepository call and a http calls to OpenAi.
    public class Monster5eRoleAdapterHelpers
    {
        // Table de référence basée sur le DMG 5e pour les statistiques par CR
        private readonly Dictionary<float, MonsterStatsByChallenge> ChallengeRatingTable = new()
        {
            // CR: (ProfBonus, AC, AvgHP, AttackBonus, DmgPerRound, SaveDC)
            [0] = new(2, 13, 4, 3, 1, 13),
            [0.125f] = new(2, 13, 9, 3, 3, 13),
            [0.25f] = new(2, 13, 14, 3, 5, 13),
            [0.5f] = new(2, 13, 20, 3, 8, 13),
            [1] = new(2, 13, 29, 3, 12, 13),
            [2] = new(2, 13, 46, 3, 18, 13),
            [3] = new(2, 14, 63, 4, 24, 13),
            [4] = new(2, 15, 84, 5, 30, 14),
            [5] = new(3, 15, 99, 6, 36, 15),
            [6] = new(3, 16, 109, 6, 42, 15),
            [7] = new(3, 16, 128, 6, 48, 15),
            [8] = new(3, 16, 135, 7, 54, 16),
            [9] = new(4, 17, 158, 7, 60, 16),
            [10] = new(4, 17, 171, 7, 66, 16),
            [11] = new(4, 17, 194, 8, 72, 17),
            [12] = new(4, 17, 190, 8, 78, 17),
            [13] = new(5, 18, 200, 8, 84, 18),
            [14] = new(5, 18, 201, 8, 90, 18),
            [15] = new(5, 18, 216, 8, 96, 18),
            [16] = new(5, 19, 240, 9, 102, 18),
            [17] = new(6, 19, 265, 10, 108, 19),
            [18] = new(6, 20, 180, 10, 114, 19),
            [19] = new(6, 20, 300, 10, 120, 19),
            [20] = new(6, 20, 331, 10, 132, 19),
            [21] = new(7, 21, 336, 11, 144, 20),
            [22] = new(7, 21, 431, 11, 156, 20),
            [23] = new(7, 22, 445, 11, 168, 20),
            [24] = new(7, 22, 546, 12, 180, 21),
            [25] = new(8, 22, 553, 12, 192, 21),
            [26] = new(8, 23, 460, 12, 204, 21),
            [27] = new(8, 23, 450, 13, 216, 22),
            [28] = new(8, 24, 540, 13, 228, 22),
            [29] = new(9, 24, 600, 13, 240, 22),
            [30] = new(9, 25, 697, 14, 252, 23)
        };

        //Minion Stat from Flee Mortals page 15
        private static List<(float Cr, int ProficiencyBonus, int HitPoints, int Damage)> GetMinionStatistics()
        {
            return new List<(float, int, int, int)>
            {
                (0, 2, 4, 1),
                (0.125f, 2, 5, 1),
                (0.25f, 2, 6, 1),
                (0.5f, 2, 7, 1),
                (1, 2, 8, 1),
                (2, 2, 9, 2),
                (3, 2, 10, 3),
                (4, 2, 11, 4),
                (5, 3, 12, 4),
                (6, 3, 13, 4),
                (7, 3, 14, 4),
                (8, 3, 15, 5),
                (9, 4, 16, 5),
                (10, 4, 17, 5),
                (11, 4, 18, 6),
                (12, 4, 19, 6),
                (13, 5, 20, 7),
                (14, 5, 21, 7),
                (15, 5, 22, 8),
                (16, 5, 23, 8),
                (17, 6, 24, 9),
                (18, 6, 25, 9),
                (19, 6, 26, 10),
                (20, 6, 27, 10),
                (21, 7, 28, 11),
                (22, 7, 29, 11),
                (23, 7, 30, 12),
                (24, 7, 31, 12),
                (25, 8, 33, 13),
                (26, 8, 34, 13),
                (27, 8, 34, 14),
                (28, 8, 35, 14),
                (29, 9, 36, 15),
                (30, 9, 37, 15)
            };
        }


        private class MonsterStatsByChallenge
        {
            public int ProfBonus { get; set; }
            public int AC { get; set; }
            public int AvgHP { get; set; }
            public int AttackBonus { get; set; }
            public int DmgPerRound { get; set; }
            public int SaveDC { get; set; }

            public MonsterStatsByChallenge(int profBonus, int ac, int avgHp, int attackBonus, int dmgPerRound, int saveDc)
            {
                ProfBonus = profBonus;
                AC = ac;
                AvgHP = avgHp;
                AttackBonus = attackBonus;
                DmgPerRound = dmgPerRound;
                SaveDC = saveDc;
            }
        }

        public class RoleAdaptationResult
        {
            public bool AlreadyFitsRole { get; set; }
            public List<string> Modifications { get; set; } = new();
            public Dictionary<string, object> OriginalValues { get; set; } = new();
            public Dictionary<string, object> NewValues { get; set; } = new();
        }

        // Méthode pour obtenir les statistiques de référence selon le CR
        private MonsterStatsByChallenge GetStatsForCR(float cr)
        {
            // Si le CR exact existe, le retourner
            if (ChallengeRatingTable.ContainsKey(cr))
                return ChallengeRatingTable[cr];

            // Sinon, trouver le CR le plus proche
            var closestCR = ChallengeRatingTable.Keys.OrderBy(k => Math.Abs(k - cr)).First();
            return ChallengeRatingTable[closestCR];
        }

        // Méthode pour calculer les seuils dynamiques basés sur le CR
        private (int low, int medium, int high) GetThresholdForStat(string statName, float cr)
        {
            var baseStats = GetStatsForCR(cr);

            switch (statName)
            {
                case "HP":
                    var hpBase = baseStats.AvgHP;
                    return ((int)(hpBase * 0.5), hpBase, (int)(hpBase * 1.5));

                case "AC":
                    var acBase = baseStats.AC;
                    return (acBase - 3, acBase, acBase + 2);

                case "Speed":
                    // La vitesse ne change pas vraiment avec le CR
                    return (20, 30, 40);

                case "Strength":
                case "Dexterity":
                case "Constitution":
                    // Les attributs augmentent avec le CR
                    var attrBase = 10 + (int)(cr / 3);
                    return (Math.Max(8, attrBase - 4), attrBase, Math.Min(20, attrBase + 4));

                case "Intelligence":
                case "Wisdom":
                case "Charisma":
                    // Les attributs mentaux augmentent moins vite
                    var mentalBase = 10 + (int)(cr / 5);
                    return (Math.Max(6, mentalBase - 4), mentalBase, Math.Min(20, mentalBase + 4));

                default:
                    return (10, 13, 16);
            }
        }

        public (RoleAdaptationResult result, Monster5eDbEntity modifiedMonster) AdaptMonsterToRole(Monster5eDbEntity monster)
        {
            var result = new RoleAdaptationResult();

            if (monster.Role == null)
            {
                result.Modifications.Add("Aucun rôle défini pour ce monstre");
                return (result, monster);
            }

            // Vérifier si le monstre correspond déjà à son rôle
            if (MonsterAlreadyFitsRole(monster))
            {
                result.AlreadyFitsRole = true;
                return (result, monster);
            }

            // Adapter selon le rôle
            switch (monster.Role)
            {
                case CombatRoleEnum.Brute:
                    AdaptToBrute(monster, result);
                    break;
                case CombatRoleEnum.Soldier:
                    AdaptToSoldier(monster, result);
                    break;
                case CombatRoleEnum.Controller:
                    AdaptToController(monster, result);
                    break;
                case CombatRoleEnum.Skirmisher:
                    AdaptToSkirmisher(monster, result);
                    break;
                case CombatRoleEnum.Ambusher:
                    AdaptToAmbusher(monster, result);
                    break;
                case CombatRoleEnum.Artillery:
                    AdaptToArtillery(monster, result);
                    break;
                case CombatRoleEnum.Minion:
                    AdaptToMinion(monster, result);
                    break;
                case CombatRoleEnum.Solo:
                    AdaptToSolo(monster, result);
                    break;
                case CombatRoleEnum.Support:
                    AdaptToSupport(monster, result);
                    break;
                case CombatRoleEnum.Leader:
                    AdaptToLeader(monster, result);
                    break;
            }

            return (result, monster);
        }

        private bool MonsterAlreadyFitsRole(Monster5eDbEntity monster)
        {
            var cr = monster.Cr;

            return monster.Role switch
            {
                CombatRoleEnum.Brute => CheckBruteFit(monster, cr),
                CombatRoleEnum.Soldier => CheckSoldierFit(monster, cr),
                CombatRoleEnum.Controller => CheckControllerFit(monster, cr),
                CombatRoleEnum.Skirmisher => CheckSkirmisherFit(monster, cr),
                CombatRoleEnum.Ambusher => CheckAmbusherFit(monster, cr),
                CombatRoleEnum.Artillery => CheckArtilleryFit(monster, cr),
                CombatRoleEnum.Minion => CheckMinionFit(monster, cr),
                CombatRoleEnum.Solo => CheckSoloFit(monster, cr),
                CombatRoleEnum.Support => CheckSupportFit(monster, cr),
                CombatRoleEnum.Leader => CheckLeaderFit(monster, cr),
                _ => false
            };
        }

        // Méthodes de vérification adaptées avec CR
        private bool CheckBruteFit(Monster5eDbEntity monster, float cr)
        {
            var hpThreshold = GetThresholdForStat("HP", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var intThreshold = GetThresholdForStat("Intelligence", cr);

            return monster.HitPoints >= hpThreshold.high &&
                   monster.Strength >= strThreshold.high &&
                   monster.Dexterity <= dexThreshold.medium &&
                   monster.Intelligence <= intThreshold.medium;
        }

        private bool CheckSoldierFit(Monster5eDbEntity monster, float cr)
        {
            var acThreshold = GetThresholdForStat("AC", cr);
            var conThreshold = GetThresholdForStat("Constitution", cr);
            var hpThreshold = GetThresholdForStat("HP", cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);

            return monster.ArmorClass >= acThreshold.high &&
                   monster.Constitution >= conThreshold.medium &&
                   monster.HitPoints >= hpThreshold.medium &&
                   monster.Dexterity <= dexThreshold.medium;
        }

        private bool CheckControllerFit(Monster5eDbEntity monster, float cr)
        {
            var intThreshold = GetThresholdForStat("Intelligence", cr);
            var wisThreshold = GetThresholdForStat("Wisdom", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);
            var conThreshold = GetThresholdForStat("Constitution", cr);

            return (monster.Intelligence >= intThreshold.medium ||
                    monster.Wisdom >= wisThreshold.medium) &&
                   monster.Strength <= strThreshold.medium &&
                   monster.Constitution <= conThreshold.medium;
        }

        private bool CheckSkirmisherFit(Monster5eDbEntity monster, float cr)
        {
            var speedThreshold = GetThresholdForStat("Speed", cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var conThreshold = GetThresholdForStat("Constitution", cr);

            var speed = ParseSpeed(monster.Speed);
            return speed >= speedThreshold.high &&
                   monster.Dexterity >= dexThreshold.medium &&
                   monster.Constitution <= conThreshold.medium;
        }

        private bool CheckAmbusherFit(Monster5eDbEntity monster, float cr)
        {
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var hpThreshold = GetThresholdForStat("HP", cr);

            return monster.Dexterity >= dexThreshold.high &&
                   monster.HitPoints <= hpThreshold.medium &&
                   monster.InitiativeBonus >= 3;
        }

        private bool CheckArtilleryFit(Monster5eDbEntity monster, float cr)
        {
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var hpThreshold = GetThresholdForStat("HP", cr);
            var acThreshold = GetThresholdForStat("AC", cr);

            return monster.Dexterity >= dexThreshold.medium &&
                   monster.HitPoints <= hpThreshold.medium &&
                   monster.ArmorClass <= acThreshold.medium;
        }

        private bool CheckMinionFit(Monster5eDbEntity monster, float cr)
        {
            var minionStats = GetMinionStatistics().FirstOrDefault(ms => ms.Cr == cr);
            return monster.HitPoints <= minionStats.HitPoints; 
        }

        private bool CheckSoloFit(Monster5eDbEntity monster, float cr)
        {
            var hpThreshold = GetThresholdForStat("HP", cr);
            var acThreshold = GetThresholdForStat("AC", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);

            return monster.HitPoints >= hpThreshold.high * 2 &&
                   monster.ArmorClass >= acThreshold.high &&
                   monster.Strength >= strThreshold.high &&
                   HasLegendaryActions(monster);
        }

        private bool CheckSupportFit(Monster5eDbEntity monster, float cr)
        {
            var wisThreshold = GetThresholdForStat("Wisdom", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);

            return monster.Wisdom >= wisThreshold.medium &&
                   monster.Strength <= strThreshold.medium &&
                   HasHealingAbilities(monster);
        }

        private bool CheckLeaderFit(Monster5eDbEntity monster, float cr)
        {
            var chaThreshold = GetThresholdForStat("Charisma", cr);
            var intThreshold = GetThresholdForStat("Intelligence", cr);

            return monster.Charisma >= chaThreshold.high &&
                   monster.Intelligence >= intThreshold.medium &&
                   HasLeadershipAbilities(monster);
        }

        // Méthodes d'adaptation avec seuils dynamiques
        private void AdaptToBrute(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var hpThreshold = GetThresholdForStat("HP", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var intThreshold = GetThresholdForStat("Intelligence", cr);

            // Brute : HP = 125-150% des HP moyens du CR
            if (monster.HitPoints < hpThreshold.high)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 1.4);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP augmentés (Brute): {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // Force élevée
            if (monster.Strength < strThreshold.high)
            {
                result.OriginalValues["Strength"] = monster.Strength;
                monster.Strength = strThreshold.high;
                result.NewValues["Strength"] = monster.Strength;
                result.Modifications.Add($"Force augmentée: {result.OriginalValues["Strength"]} → {monster.Strength}");
            }

            // Dextérité faible
            if (monster.Dexterity > dexThreshold.medium)
            {
                result.OriginalValues["Dexterity"] = monster.Dexterity;
                monster.Dexterity = dexThreshold.low;
                result.NewValues["Dexterity"] = monster.Dexterity;
                result.Modifications.Add($"Dextérité réduite: {result.OriginalValues["Dexterity"]} → {monster.Dexterity}");
            }

            // Intelligence faible
            if (monster.Intelligence > intThreshold.low)
            {
                result.OriginalValues["Intelligence"] = monster.Intelligence;
                monster.Intelligence = intThreshold.low;
                result.NewValues["Intelligence"] = monster.Intelligence;
                result.Modifications.Add($"Intelligence réduite: {result.OriginalValues["Intelligence"]} → {monster.Intelligence}");
            }

            // AC légèrement réduite (brutes moins défensives)
            if (monster.ArmorClass > baseStats.AC - 1)
            {
                result.OriginalValues["ArmorClass"] = monster.ArmorClass;
                monster.ArmorClass = baseStats.AC - 1;
                result.NewValues["ArmorClass"] = monster.ArmorClass;
                result.Modifications.Add($"CA ajustée: {result.OriginalValues["ArmorClass"]} → {monster.ArmorClass}");
            }
        }

        private void AdaptToSoldier(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var acThreshold = GetThresholdForStat("AC", cr);
            var conThreshold = GetThresholdForStat("Constitution", cr);
            var hpThreshold = GetThresholdForStat("HP", cr);

            // Soldier : AC élevée
            if (monster.ArmorClass < acThreshold.high)
            {
                result.OriginalValues["ArmorClass"] = monster.ArmorClass;
                monster.ArmorClass = baseStats.AC + 2;
                result.NewValues["ArmorClass"] = monster.ArmorClass;
                result.Modifications.Add($"CA augmentée (Soldier): {result.OriginalValues["ArmorClass"]} → {monster.ArmorClass}");
            }

            // Constitution élevée
            if (monster.Constitution < conThreshold.high)
            {
                result.OriginalValues["Constitution"] = monster.Constitution;
                monster.Constitution = conThreshold.high;
                result.NewValues["Constitution"] = monster.Constitution;
                result.Modifications.Add($"Constitution augmentée: {result.OriginalValues["Constitution"]} → {monster.Constitution}");
            }

            // HP moyens à élevés (100-120% du CR)
            if (monster.HitPoints < baseStats.AvgHP)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 1.1);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP ajustés: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // Vitesse réduite
            if (ParseSpeed(monster.Speed) > 25)
            {
                result.OriginalValues["Speed"] = monster.Speed;
                monster.Speed = "25 ft.";
                result.NewValues["Speed"] = monster.Speed;
                result.Modifications.Add($"Vitesse réduite: {result.OriginalValues["Speed"]} → {monster.Speed}");
            }
        }

        private void AdaptToController(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var intThreshold = GetThresholdForStat("Intelligence", cr);
            var wisThreshold = GetThresholdForStat("Wisdom", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);
            var hpThreshold = GetThresholdForStat("HP", cr);

            // Intelligence ou Sagesse élevée
            if (monster.Intelligence < intThreshold.medium && monster.Wisdom < wisThreshold.medium)
            {
                result.OriginalValues["Intelligence"] = monster.Intelligence;
                monster.Intelligence = intThreshold.high;
                result.NewValues["Intelligence"] = monster.Intelligence;
                result.Modifications.Add($"Intelligence augmentée (Controller): {result.OriginalValues["Intelligence"]} → {monster.Intelligence}");
            }

            // Force faible
            if (monster.Strength > strThreshold.medium)
            {
                result.OriginalValues["Strength"] = monster.Strength;
                monster.Strength = strThreshold.low;
                result.NewValues["Strength"] = monster.Strength;
                result.Modifications.Add($"Force réduite: {result.OriginalValues["Strength"]} → {monster.Strength}");
            }

            // HP réduits (70-80% du CR)
            if (monster.HitPoints > baseStats.AvgHP * 0.8)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 0.75);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP réduits: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // Save DC élevé pour les sorts de contrôle
            var expectedSaveDC = baseStats.SaveDC + 1;
            result.Modifications.Add($"Save DC recommandé pour les sorts: {expectedSaveDC}");
        }

        private void AdaptToSkirmisher(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var speedThreshold = GetThresholdForStat("Speed", cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var conThreshold = GetThresholdForStat("Constitution", cr);

            // Vitesse élevée
            var currentSpeed = ParseSpeed(monster.Speed);
            if (currentSpeed < speedThreshold.high)
            {
                result.OriginalValues["Speed"] = monster.Speed;
                monster.Speed = "40 ft.";
                result.NewValues["Speed"] = monster.Speed;
                result.Modifications.Add($"Vitesse augmentée (Skirmisher): {result.OriginalValues["Speed"]} → {monster.Speed}");
            }

            // Dextérité élevée
            if (monster.Dexterity < dexThreshold.high)
            {
                result.OriginalValues["Dexterity"] = monster.Dexterity;
                monster.Dexterity = dexThreshold.high;
                result.NewValues["Dexterity"] = monster.Dexterity;
                result.Modifications.Add($"Dextérité augmentée: {result.OriginalValues["Dexterity"]} → {monster.Dexterity}");
            }

            // Constitution faible
            if (monster.Constitution > conThreshold.medium)
            {
                result.OriginalValues["Constitution"] = monster.Constitution;
                monster.Constitution = conThreshold.low;
                result.NewValues["Constitution"] = monster.Constitution;
                result.Modifications.Add($"Constitution réduite: {result.OriginalValues["Constitution"]} → {monster.Constitution}");
            }

            // HP réduits (75-85% du CR)
            if (monster.HitPoints > baseStats.AvgHP * 0.85)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 0.8);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP ajustés: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }
        }

        private void AdaptToAmbusher(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);

            // Dextérité très élevée
            if (monster.Dexterity < dexThreshold.high)
            {
                result.OriginalValues["Dexterity"] = monster.Dexterity;
                monster.Dexterity = Math.Min(20, dexThreshold.high + 2);
                result.NewValues["Dexterity"] = monster.Dexterity;
                result.Modifications.Add($"Dextérité augmentée (Ambusher): {result.OriginalValues["Dexterity"]} → {monster.Dexterity}");
            }

            // Initiative bonus élevé
            var expectedInitBonus = ((monster.Dexterity ?? 10) - 10) / 2 + 2;
            if (monster.InitiativeBonus < expectedInitBonus)
            {
                result.OriginalValues["InitiativeBonus"] = monster.InitiativeBonus;
                monster.InitiativeBonus = expectedInitBonus;
                result.NewValues["InitiativeBonus"] = monster.InitiativeBonus;
                result.Modifications.Add($"Initiative augmentée: {result.OriginalValues["InitiativeBonus"]} → {monster.InitiativeBonus}");
            }

            // HP très faibles (60-70% du CR)
            if (monster.HitPoints > baseStats.AvgHP * 0.7)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 0.65);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP réduits: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // AC moyenne
            if (monster.ArmorClass > baseStats.AC)
            {
                result.OriginalValues["ArmorClass"] = monster.ArmorClass;
                monster.ArmorClass = baseStats.AC;
                result.NewValues["ArmorClass"] = monster.ArmorClass;
                result.Modifications.Add($"CA ajustée: {result.OriginalValues["ArmorClass"]} → {monster.ArmorClass}");
            }
        }

        private void AdaptToArtillery(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);

            // Dextérité élevée pour la précision
            if (monster.Dexterity < dexThreshold.high)
            {
                result.OriginalValues["Dexterity"] = monster.Dexterity;
                monster.Dexterity = dexThreshold.high;
                result.NewValues["Dexterity"] = monster.Dexterity;
                result.Modifications.Add($"Dextérité augmentée (Artillery): {result.OriginalValues["Dexterity"]} → {monster.Dexterity}");
            }

            // HP très faibles (50-60% du CR)
            if (monster.HitPoints > baseStats.AvgHP * 0.6)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 0.55);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP réduits: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // AC faible
            if (monster.ArmorClass > baseStats.AC - 2)
            {
                result.OriginalValues["ArmorClass"] = monster.ArmorClass;
                monster.ArmorClass = baseStats.AC - 2;
                result.NewValues["ArmorClass"] = monster.ArmorClass;
                result.Modifications.Add($"CA réduite: {result.OriginalValues["ArmorClass"]} → {monster.ArmorClass}");
            }

            // Force très faible
            if (monster.Strength > strThreshold.low)
            {
                result.OriginalValues["Strength"] = monster.Strength;
                monster.Strength = strThreshold.low;
                result.NewValues["Strength"] = monster.Strength;
                result.Modifications.Add($"Force réduite: {result.OriginalValues["Strength"]} → {monster.Strength}");
            }

            // Bonus d'attaque à distance élevé
            var expectedAttackBonus = baseStats.AttackBonus + 2;
            result.Modifications.Add($"Bonus d'attaque à distance recommandé: +{expectedAttackBonus}");
        }

        private void AdaptToMinion(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var minionStats = GetMinionStatistics().First(ms => ms.Cr == monster.Cr);
            // Les minions ont toujours 1 HP
            if (monster.HitPoints != 1)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = minionStats.HitPoints;
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP fixés à 1 (Minion)");
            }

            // AC spéciale pour les minions (AC de base - 2)
            var baseStats = GetStatsForCR(monster.Cr);
            if (monster.MinionArmorClass == null)
            {
                var minionCrMalus = new Dice(1, 4).Roll -1;
                monster.MinionArmorClass = baseStats.AC - minionCrMalus;
                result.Modifications.Add($"CA de Minion définie: {monster.MinionArmorClass} (base AC {baseStats.AC}-{minionCrMalus})");
            }

            //Damages par round
            var expectedDamage = minionStats.Damage;
            result.Modifications.Add($"Dégâts par round recommandés: {expectedDamage}");

            // Réduire toutes les stats
            ReduceAllStats(monster, result, 0.7f);
        }

        private void AdaptToSolo(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);

            // HP massifs (250-300% du CR)
            if (monster.HitPoints < baseStats.AvgHP * 2.5)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 2.75);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP augmentés (Solo): {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // AC élevée
            if (monster.ArmorClass < baseStats.AC + 2)
            {
                result.OriginalValues["ArmorClass"] = monster.ArmorClass;
                monster.ArmorClass = baseStats.AC + 3;
                result.NewValues["ArmorClass"] = monster.ArmorClass;
                result.Modifications.Add($"CA augmentée: {result.OriginalValues["ArmorClass"]} → {monster.ArmorClass}");
            }

            // Augmenter toutes les stats de combat
            BoostAllCombatStats(monster, result, cr);

            // Actions légendaires (3 par round)
            result.Modifications.Add("Actions légendaires recommandées: 3 par round");
            result.Modifications.Add("Résistances légendaires recommandées: 3 par jour");
        }

        private void AdaptToSupport(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var wisThreshold = GetThresholdForStat("Wisdom", cr);
            var strThreshold = GetThresholdForStat("Strength", cr);

            // Sagesse élevée
            if (monster.Wisdom < wisThreshold.high)
            {
                result.OriginalValues["Wisdom"] = monster.Wisdom;
                monster.Wisdom = wisThreshold.high;
                result.NewValues["Wisdom"] = monster.Wisdom;
                result.Modifications.Add($"Sagesse augmentée (Support): {result.OriginalValues["Wisdom"]} → {monster.Wisdom}");
            }

            // Force faible
            if (monster.Strength > strThreshold.low)
            {
                result.OriginalValues["Strength"] = monster.Strength;
                monster.Strength = strThreshold.low;
                result.NewValues["Strength"] = monster.Strength;
                result.Modifications.Add($"Force réduite: {result.OriginalValues["Strength"]} → {monster.Strength}");
            }

            // HP moyens (80-90% du CR)
            if (monster.HitPoints > baseStats.AvgHP * 0.9)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 0.85);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP ajustés: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            // Capacités de soin basées sur le CR
            var healingAmount = Math.Max(4, baseStats.DmgPerRound / 3);
            result.Modifications.Add($"Soins recommandés par action: {healingAmount} HP");
            result.Modifications.Add("Actions recommandées: Soins, Bénédiction, Protection");
        }

        private void AdaptToLeader(Monster5eDbEntity monster, RoleAdaptationResult result)
        {
            var cr = monster.Cr;
            var baseStats = GetStatsForCR(cr);
            var chaThreshold = GetThresholdForStat("Charisma", cr);
            var intThreshold = GetThresholdForStat("Intelligence", cr);

            // Charisme très élevé
            if (monster.Charisma < chaThreshold.high)
            {
                result.OriginalValues["Charisma"] = monster.Charisma;
                monster.Charisma = Math.Min(20, chaThreshold.high + 2);
                result.NewValues["Charisma"] = monster.Charisma;
                result.Modifications.Add($"Charisme augmenté (Leader): {result.OriginalValues["Charisma"]} → {monster.Charisma}");
            }

            // Intelligence élevée
            if (monster.Intelligence < intThreshold.medium)
            {
                result.OriginalValues["Intelligence"] = monster.Intelligence;
                monster.Intelligence = intThreshold.high;
                result.NewValues["Intelligence"] = monster.Intelligence;
                result.Modifications.Add($"Intelligence augmentée: {result.OriginalValues["Intelligence"]} → {monster.Intelligence}");
            }

            // HP moyens à élevés (110-120% du CR)
            if (monster.HitPoints < baseStats.AvgHP * 1.1)
            {
                result.OriginalValues["HitPoints"] = monster.HitPoints;
                monster.HitPoints = (int)(baseStats.AvgHP * 1.15);
                result.NewValues["HitPoints"] = monster.HitPoints;
                result.Modifications.Add($"HP ajustés: {result.OriginalValues["HitPoints"]} → {monster.HitPoints}");
            }

            result.Modifications.Add("Actions recommandées: Commandement, Inspiration, Tactiques de groupe");
            result.Modifications.Add($"Bonus aux alliés recommandé: +{baseStats.ProfBonus} aux jets d'attaque ou de sauvegarde");
        }

        // Méthodes utilitaires mises à jour
        private void BoostAllCombatStats(Monster5eDbEntity monster, RoleAdaptationResult result, float cr)
        {
            var strThreshold = GetThresholdForStat("Strength", cr);
            var dexThreshold = GetThresholdForStat("Dexterity", cr);
            var conThreshold = GetThresholdForStat("Constitution", cr);

            if (monster.Strength < strThreshold.high)
            {
                result.OriginalValues["Strength"] = monster.Strength;
                monster.Strength = Math.Min(20, strThreshold.high + 2);
                result.NewValues["Strength"] = monster.Strength;
            }

            if (monster.Dexterity < dexThreshold.medium)
            {
                result.OriginalValues["Dexterity"] = monster.Dexterity;
                monster.Dexterity = dexThreshold.high;
                result.NewValues["Dexterity"] = monster.Dexterity;
            }

            if (monster.Constitution < conThreshold.high)
            {
                result.OriginalValues["Constitution"] = monster.Constitution;
                monster.Constitution = Math.Min(20, conThreshold.high + 2);
                result.NewValues["Constitution"] = monster.Constitution;
            }

            // Améliorer les sauvegardes basées sur le proficiency bonus
            var baseStats = GetStatsForCR(cr);
            var profBonus = baseStats.ProfBonus;

            monster.StrSavingThrow = ((monster.Strength ?? 10) - 10) / 2 + profBonus;
            monster.ConSavingThrow = ((monster.Constitution ?? 10) - 10) / 2 + profBonus;
            monster.WisSavingThrow = ((monster.Wisdom ?? 10) - 10) / 2 + profBonus;
        }

        private void ReduceAllStats(Monster5eDbEntity monster, RoleAdaptationResult result, float factor)
        {
            if (monster.Strength > 8)
            {
                result.OriginalValues["Strength"] = monster.Strength;
                monster.Strength = Math.Max(6, (int)((monster.Strength ?? 10) * factor));
                result.NewValues["Strength"] = monster.Strength;
            }

            if (monster.Dexterity > 8)
            {
                result.OriginalValues["Dexterity"] = monster.Dexterity;
                monster.Dexterity = Math.Max(6, (int)((monster.Dexterity ?? 10) * factor));
                result.NewValues["Dexterity"] = monster.Dexterity;
            }

            if (monster.Constitution > 8)
            {
                result.OriginalValues["Constitution"] = monster.Constitution;
                monster.Constitution = Math.Max(6, (int)((monster.Constitution ?? 10) * factor));
                result.NewValues["Constitution"] = monster.Constitution;
            }
        }

        private int ParseSpeed(string? speed)
        {
            if (string.IsNullOrEmpty(speed)) return 30;
            var match = System.Text.RegularExpressions.Regex.Match(speed, @"\d+");
            return match.Success ? int.Parse(match.Value) : 30;
        }

        private bool HasLegendaryActions(Monster5eDbEntity monster)
        {
            return monster.Actions?.Any(a => a.Type == ActionTypeEnum.Legendary) ?? false;
        }

        private bool HasHealingAbilities(Monster5eDbEntity monster)
        {
            //TODO : Ask ChatGPT
            return false;
        }

        private bool HasLeadershipAbilities(Monster5eDbEntity monster)
        {
            //TODO : Ask ChatGPT
            return false;
        }
    }
}
