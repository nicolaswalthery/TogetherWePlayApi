using Common.Extensions;
using Common.ResultPattern;
using System.Text.Json;
using TWP.Api.Application.DataEnrichmentLayer.Interfaces;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.Helpers;
using TWP.Api.Infrastructure.Helpers;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.DataEnrichmentLayer
{
    public class Monster5eDataEnrichmentLayer : IMonster5eDataEnrichmentLayer
    {

        private readonly string _promptLore = "Give me the lore of this dnd5e monster according to your knowledge.";
        private readonly string _promptManner = "You are a masterful D&D 5e monster designer and narrator. For any given monster, you will generate a short and evocative phrase that captures its manner, behavior, or presence — something a Game Master would read aloud to players as they first encounter the creature.\r\n\r\nThe phrase must:\r\n- Be **10 to 15 words** maximum\r\n- Use **evocative verbs** and **sensory language**\r\n- Convey the **emotion or intent** of the creature (e.g., fear, rage, hunger)\r\n- Focus on **actions or body language**, not stats or abilities\r\n- Start with a capital letter and **no period at the end**\r\n\r\nExample:\r\n- *Manner clacks fiercely with its mandibles*\r\n- *The beast puffs up, emitting a sickly green mist*\r\n- *It hisses and drags its claws across the stone*\r\n\r\nNow generate a phrase for this monster: ";
        private readonly string _promptRole = "You are a master of D&D 5e monster design. Based on the provided monster stat block or description, determine which of the following **Combat Roles** best fits the creature's behavior and tactical function in combat.\r\n\r\nYou must choose **only one** of the following roles:\r\n\r\n- Ambusher\r\n- Artillery\r\n- Brute\r\n- Controller\r\n- Leader\r\n- Minion\r\n- Skirmisher\r\n- Soldier\r\n- Solo\r\n- Support\r\n\r\n### Instructions:\r\n- Read the monster description, stat block, or traits.\r\n- Choose the most accurate **Combat Role** from the list.\r\n- Respond with the **exact name of the chosen role**, and **nothing else**.\r\n- Do not explain, justify, rephrase, or format your answer.\r\n\r\nNow determine the combat role for this monster:\r\n\r\n[Insert monster stat block or description here]\r\n";
        private readonly IOpenAiInterops _openAiInterops;
        private readonly IMonster5eRepository _monster5ERepository;

        public Monster5eDataEnrichmentLayer(IOpenAiInterops openAiInterops, IMonster5eRepository monster5ERepository)
        {
            _openAiInterops = openAiInterops;
            _monster5ERepository = monster5ERepository;
        }

        public async Task<Result> AiRoleDetermination()
           => await Safe.ExecuteAsync(async () =>
           {
               var roleDescriptions = RoleDescriptionsHelper.GetAllRoleDescriptions();
               var result = await _monster5ERepository.GetAllAsync();
               //foreach (var monster in result.Data.Where(m => m.Role is null))
               //{
               //    var role = await _openAiInterops.ChatGptResponseAsync($"Role description : {roleDescriptions} -> Monster Stats {monster.ToFullString()} -> {_promptRole}");
               //    monster.Role = role.GetCombatRoleFromString();

               //    await _monster5ERepository.Update(monster);
               //}

               return Result.Success();
           });

        public async Task<Result> AiLoreAndMannerDetermination()
           => await Safe.ExecuteAsync(async () =>
           {
               var result = await _monster5ERepository.GetAllAsync();
               foreach (var monster in result.Data)
               {
                   var lore = await _openAiInterops.ChatGptResponseAsync($"{ _promptLore} {monster.Name}");
                   var manner = await _openAiInterops.ChatGptResponseAsync($"{_promptManner} {monster.Name}");

                   monster.Lore = JsonSerializer.Serialize(new { value = lore });
                   monster.Manner = JsonSerializer.Serialize(new { value = manner });

                   await _monster5ERepository.Update(monster);
               }

               return Result.Success();
           });
    }
}
