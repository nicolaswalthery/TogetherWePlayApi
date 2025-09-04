using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eMonsterBusinessLayer : IDnd5eMonsterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _csvRepository;
        private readonly IMonster5eRepository _monster5ERepository;
        private readonly IMonsterBuildingGuidelineRepository _monsterBuildingGuidelineRepository;

        public Dnd5eMonsterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository csvRepository, IMonster5eRepository monster5ERepository, IMonsterBuildingGuidelineRepository monsterBuildingGuidelineRepository)
        {
            _csvRepository = csvRepository;
            _monster5ERepository = monster5ERepository;
            _monsterBuildingGuidelineRepository = monsterBuildingGuidelineRepository;
        }

        public async Task<Result<Monster5eDto>> CreateOriginalDndMonster(float challengeRating)
            => await Safe.ExecuteAsync(async () =>
            {
                //TODO
                //Pick a base monster from dnd2024
                var baseMonsters = await _monster5ERepository.FindByCrAsync(challengeRating);
                if (baseMonsters.IsFailure)
                    return Result<Monster5eDto>.Failure("No Monsters found for the given CR", ReasonType.NotFound);
                var randomSelector = new RandomSelector<Monster5eDbEntity>();
                var baseMonster = randomSelector.SelectOneRandomly(baseMonsters.Data.ToArray());

                //Pick Role
                var role = EnumExtensions.GetRandomElementOfEnum<CombatRoleEnum>();
                var roleDescription = GetRoleDescription(role);


                //Generate monster lore from base monster
                var baseLore = baseMonster.Lore;

                //Get Guide line
                //var guideLineForCr = _monsterBuildingGuidelineRepository.GetByCRAsync(baseMonster.Cr);

                //Pick Traits

                //Pick Action
                throw new NotImplementedException();
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
                if(results.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(results.Error!, results.ReasonType);
                return results;
            });

        /// <summary>
        /// Returns the description of a combat role based on the provided enum value.
        /// </summary>
        /// <param name="role">The combat role enum value</param>
        /// <returns>A string description of the combat role</returns>
        private static string GetRoleDescription(CombatRoleEnum role)
        {
            return role switch
            {
                CombatRoleEnum.Ambusher =>
                    "Ambushers are creatures who hide well—not just before an encounter, but during it. They utilize surprise and stealth to gain the upper hand. They typically attack a player character, then slip away to other cover, slipping and hiding. They focus on taking down a single character, sometimes dragging their target into the place where they hide.",

                CombatRoleEnum.Artillery =>
                    "Artillery creatures fight best from afar. Whether they wield arrows or magical rays, these creatures always try to keep a distance from their foes. They are great at ranged combat and can damage player characters who typically hang back behind their counterparts. Most artillery are weak in melee, so add some brutes, minions, or soldiers for them to hide behind during combat.",

                CombatRoleEnum.Brute =>
                    "Brutes are hardy creatures who have lots of hit points and deal lots of damage. They might not be the most disciplined warriors, but they make up for it with sheer toughness and aggression. They hit hard and have a lot of hit points. Their damage output can't be ignored, so player characters often focus on taking them down instead of other creatures who have fewer hit points or deal less damage.",

                CombatRoleEnum.Controller =>
                    "Controllers debuff, move, and obstruct their enemies. They often have crowd control actions that apply a debilitating effect or target multiple creatures at once. With their ability to debuff, hamper, and move player characters, controllers make a battle more dynamic. They tend to have more complicated actions and traits with unique effects, so most combat encounters shouldn't have more than one or two controllers.",

                CombatRoleEnum.Leader =>
                    "A leader is an action-oriented creature who fights alongside underlings. Leaders are action-oriented creatures and fight best alongside allies. Leaders can summon reinforcements, buff their allies, and grant allies extra movement and actions. As long as a leader stays in the fight, their allies are enhanced. They are most effective when protected by and buffing artillery, brutes, minions, skirmishers, and soldiers.",

                CombatRoleEnum.Minion =>
                    "Minions are weak creatures who find strength in numbers. They allow you to create cinematic battles where the characters feel heroic as they cut through several foes at a time. Using minions of a challenge rating within 2 of the characters' average level also keeps them dangerous and relevant. When minions work together, they can't be ignored, as they deal a lot of damage and can lock down the characters.",

                CombatRoleEnum.Skirmisher =>
                    "Skirmishers are mobile warriors who use hit-and-run tactics in combat. Their traits allow them to make the most of their position. They can move to attack vulnerable player characters who are weaker in melee, then retreat if they can do so safely to protect their artillery, controller, leader, and support allies. Since they can outrun and outmaneuver player characters, this forces the heroes to act tactically.",

                CombatRoleEnum.Soldier =>
                    "Soldiers are well-armored creatures who draw the attacks of their foes, freeing allies to move around the battlefield. These trained warriors typically have higher attack bonuses and AC. They defend their allies by drawing the player characters' attacks. They make excellent protection for ambushers, artillery creatures, controllers, leaders, and support creatures as they direct attention toward themselves.",

                CombatRoleEnum.Solo =>
                    "A solo creature is an action-oriented creature who can take on the player characters on their own. Solo creatures are action-oriented and their villain actions and legendary actions allow them to act outside of the normal turn order. They're designed alone and fight most effectively in encounters with plenty of space to move around and find cover. Solo creatures have every movement advantage they can have, including burrowing, climbing, flying, swimming, or teleportation.",

                CombatRoleEnum.Support =>
                    "Support creatures aid their allies, providing buffs, healing, movement, or action options. They stay close to their allies so their features can benefit as many creatures as possible. They often use their biggest and most powerful effects at the start of an encounter, affecting as many foes as possible. Leaders and support creatures remain close to their allies so their features can benefit as many creatures as possible.",

                _ => "Unknown combat role."
            };
        }
    }
} 