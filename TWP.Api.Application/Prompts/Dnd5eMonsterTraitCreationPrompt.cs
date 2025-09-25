using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.Prompts
{
    public static class Dnd5eMonsterTraitCreationPrompt
    {
        public static string GetDnd5eMonsterTraitCreationPrompt(this Monster5eDbEntity monster) 
            => @$"Crée un trait de monstre D&D 5e et retourne le résultat en JSON pour l'objet TraitDbEntity suivant :

            **CONTEXTE DU MONSTRE**
            - Nom : {monster.Name}
            - Type de créature : {monster.CreatureType} {monster.CreatureSubType}
            - CR (Challenge Rating) : {monster.Cr}
            - Taille : {monster.CreatureSize}
            - Rôle au combat : {monster.Role}
            - Classe d'armure : {monster.ArmorClass}
            - Points de vie : {monster.HitPoints}
            - Thème/concept : [crée une description brève basé sur {monster.Lore}]

            **TYPE DE TRAIT SOUHAITÉ** (choisis-en un approprié au CR et au rôle)
            - Pour CR 1-5 : Traits simples (Pack Tactics, Keen Senses, Amphibious)
            - Pour CR 6-10 : Traits modérés (Resistance, Regeneration limitée, Auras simples)
            - Pour CR 11-16 : Traits complexes (Legendary Resistance, Auras puissantes)
            - Pour CR 17+ : Traits légendaires (Resistances multiples, Régénération majeure)

            **FORMAT DE SORTIE : JSON**
            {{
                ""monsterId"": ""00000000-0000-0000-0000-000000000000"",
                ""title"": ""Nom du Trait"",
                ""description"": ""Description complète du trait selon le format D&D 5e"",
                ""attackBonus"": null,
                ""damageBonus"": null,
                ""damageDice"": null,
                ""numberDamageDice"": null,
                ""damageType"": null,
                ""traitTrigger"": null,
                ""advantageCondition"": null,
                ""disadvantageCondition"": null,
                ""isOptional"": false
            }}

            **ENUMS DE RÉFÉRENCE:**
            DiceTypeEnum: d2=2, d3=3, d4=4, d5=5, d6=6, d7=7, d8=8, d9=9, d10=10, d11=11, d12=12, d13=13, d14=14, d15=15, d16=16, d17=17, d18=18, d19=19, d20=20, d66=66, d100=100

            DamageTypeEnum: 1=Acid, 2=Bludgeoning, 3=Cold, 4=Fire, 5=Force, 6=Lightning, 7=Necrotic, 8=Piercing, 9=Poison, 10=Psychic, 11=Radiant, 12=Slashing, 13=Thunder

            **CONTRAINTES:**
            - Le trait doit être équilibré pour le CR du monstre
            - Les dégâts doivent être appropriés (1d6 par tranche de CR 5 environ)
            - DD de sauvegarde = 8 + Bonus de maîtrise + Modificateur de caractéristique
            - Description au format standard D&D 5e
            - Les valeurs null doivent être explicites dans le JSON

            **CONTRAINTES RÉALISTES (SCI-FI):**
            - PAS de magie, uniquement technologie et biologie avancée
            - Effets basés sur : technologie, mutations, cybernétique, nanotechnologie
            - Conditions réalistes : aveuglé (flash), étourdi (EMP), ralenti (gel cryogénique)
            - Résistances technologiques plutôt que magiques

            **EXEMPLE DE SORTIE JSON:**
            {{
              ""monsterId"": ""00000000-0000-0000-0000-000000000000"",
              ""title"": ""Adaptive Armor"",
              ""description"": ""When the creature takes damage, it gains resistance to that damage type until the start of its next turn."",
              ""attackBonus"": null,
              ""damageBonus"": null,
              ""damageDice"": null,
              ""numberDamageDice"": null,
              ""damageType"": null,
              ""traitTrigger"": ""when taking damage"",
              ""advantageCondition"": null,
              ""disadvantageCondition"": null,
              ""isOptional"": false
            }}";
    }
}
