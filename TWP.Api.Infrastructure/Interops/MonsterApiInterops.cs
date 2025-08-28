using Newtonsoft.Json;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.Interops.Interfaces;

namespace TWP.Api.Infrastructure.Interops
{
    public class MonsterApiInterops : IMonsterApiInterops
    {
        private readonly HttpClient _httpClient;

        public MonsterApiInterops(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MonsterApiResponseDto> GetMonsterDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.dnd5eapi.co/api/2014/monsters");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MonsterApiResponseDto>(result);
        }

        public async Task<SpellApiResponseDto> GetSpellDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.dnd5eapi.co/api/2014/spells");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<SpellApiResponseDto>(result);
        }

        public async Task<MonsterApiResponseDto> GetMonstersByChallengeRatingAsync(int cr)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/monsters?challenge_rating={cr}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MonsterApiResponseDto>(result);
        }
    }
}
