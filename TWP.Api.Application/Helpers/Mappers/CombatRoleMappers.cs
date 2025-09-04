using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Helpers.Mappers
{
    public static class CombatRoleMapper
    {
        public static CombatRoleEnum? GetCombatRoleFromString(this string role)
        {
            return role?.ToLower() switch
            {
                "brute" => CombatRoleEnum.Brute,
                "soldier" => CombatRoleEnum.Soldier,
                "controller" => CombatRoleEnum.Controller,
                "skirmisher" => CombatRoleEnum.Skirmisher,
                "ambusher" => CombatRoleEnum.Ambusher,
                "artillery" => CombatRoleEnum.Artillery,
                "minion" => CombatRoleEnum.Minion,
                "solo" => CombatRoleEnum.Solo,
                "support" => CombatRoleEnum.Support,
                "leader" => CombatRoleEnum.Leader,
                _ => null // Retourne null si la chaîne ne correspond à aucun rôle valide
            };
        }

        public static string GetStringFromCombatRole(this CombatRoleEnum role)
        {
            return role switch
            {
                CombatRoleEnum.Brute => "Brute",
                CombatRoleEnum.Soldier => "Soldier",
                CombatRoleEnum.Controller => "Controller",
                CombatRoleEnum.Skirmisher => "Skirmisher",
                CombatRoleEnum.Ambusher => "Ambusher",
                CombatRoleEnum.Artillery => "Artillery",
                CombatRoleEnum.Minion => "Minion",
                CombatRoleEnum.Solo => "Solo",
                CombatRoleEnum.Support => "Support",
                CombatRoleEnum.Leader => "Leader",
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }
    }
}

