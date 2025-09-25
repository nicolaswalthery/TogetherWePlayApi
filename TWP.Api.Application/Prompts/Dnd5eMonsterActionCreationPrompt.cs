using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.Prompts
{
    public static class Dnd5eMonsterActionCreationPrompt
    {
        public static string GetDnd5eMonsterActionCreationPrompt(this Monster5eDbEntity monster) =>
            @$"Crée une action pour un monstre D&D 5e avec les paramètres suivants :

            INFORMATIONS DE BASE :
            - Nom du monstre : {monster.Name}
            - CR (Challenge Rating) : {monster.Cr}
            - Type de créature : {monster.CreatureType} {monster.CreatureSubType}
            - Rôle tactique : {monster.Role}
            - Thème/concept : [crée une description brève bésé sur {monster.Lore}]

            ACTION À CRÉER :
            - Type d'action : [Choisi un : Action/Action Bonus/Réaction/Légendaire]
            - Objectif tactique : [Choisi un : dégâts/contrôle/mobilité/débuff/autre]
            - Fréquence : [Choisi un : à volonté/recharge/X fois par jour]

            FORMAT DE SORTIE :
            Retourne un objet JSON pour l'entité ActionDbEntity avec les propriétés suivantes :
            - MonsterId : GUID (utilise ""00000000-0000-0000-0000-000000000000"" comme placeholder)
            - Name : string (nom de l'action)
            - Type : enum (Action=0, Bonus=1, Reaction=2, Legendary=3, Movement=4, Lair=5, Special=6)
            - AttackType : enum (MeleeWeaponAttack=0, RangedWeaponAttack=1, MeleeSpellAttack=2, RangedSpellAttack=3, SavingThrow=4, Automatic=5)
            - Description : string (description complète de l'action au format D&D 5e)
            - ShortRange : string? (portée courte en pieds, null si non applicable)
            - LongRange : string? (portée longue en pieds, null si non applicable)
            - AttackBonus : int? (bonus d'attaque, null si jet de sauvegarde)
            - DamageBonus : int? (bonus aux dégâts fixes)
            - DamageDice : enum? (D4=4, D6=6, D8=8, D10=10, D12=12, D20=20, D100=100)
            - NumberDamageDice : int? (nombre de dés de dégâts)
            - DamageType : enum? (Acid=0, Bludgeoning=1, Cold=2, Fire=3, Force=4, Lightning=5, Necrotic=6, Piercing=7, Poison=8, Psychic=9, Radiant=10, Slashing=11, Thunder=12)
            - LimitPerDay : int? (null pour à volonté, nombre pour X/jour)
            - IsProhibitedForMinion : bool (true si l'action est trop puissante pour une version minion)
            - actionTrigger : string? (condition de déclenchement pour les réactions, null sinon)
            - advantageCondition : string? (condition donnant l'avantage sur cette action)
            - disadvantageCondition : string? (condition donnant le désavantage sur cette action)

            CONTRAINTES :
            - Bonus d'attaque = Bonus de maîtrise + Modificateur de caractéristique
            - DD de sauvegarde = 8 + Bonus de maîtrise + Modificateur
            - Dégâts appropriés au CR
            - Description doit suivre le format standard D&D 5e
            - Les valeurs null doivent être explicites dans le JSON

            CONTRAINTES RÉALISTES :
            - PAS de magie, pouvoirs psychiques, télépathie, ou ""space magic""
            - Uniquement : armes à feu, explosifs, armes de mêlée, technologie plausible
            - Effets basés sur : physique, chimie, biologie, technologie crédible
            - Les ""conditions"" doivent être réalistes : aveuglé (flash/fumée), étourdi (commotion), ralenti (blessure), etc.
            - Portées réalistes pour les armes (pistolet 30/120 ft, fusil 150/600 ft, etc.)


            EXEMPLE DE SORTIE JSON :
            {{
              ""MonsterId"": ""00000000-0000-0000-0000-000000000000"",
              ""Name"": ""Fire Breath"",
              ""Type"": 0,
              ""AttackType"": 4,
              ""Description"": ""Fire Breath (Recharge 5-6). Dexterity Saving Throw: DC 17, each creature in a 30-foot Cone. Failure: 56 (16d6) Fire damage. Success: Half damage."",
              ""ShortRange"": ""30"",
              ""LongRange"": null,
              ""AttackBonus"": null,
              ""DamageBonus"": null,
              ""DamageDice"": 6,
              ""NumberDamageDice"": 16,
              ""DamageType"": 3,
              ""LimitPerDay"": null,
              ""IsProhibitedForMinion"": true,
              ""actionTrigger"": null,
              ""advantageCondition"": null,
              ""disadvantageCondition"": null
            }}";
    }
}
