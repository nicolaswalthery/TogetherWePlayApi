using Common.Extensions;
using Newtonsoft.Json;
using TWP.Api.Application.Interfaces.Services;
using TWP.Api.Core.DataTransferObjects;

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
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/monsters?challenge_rating<={cr}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MonsterApiResponseDto>(result);
        }

        public async Task<MonsterApiResponseDto> GetMonstersByChallengeRatingOrlessAsync(int cr)
        {
            var challengeRatings = GetCrUntil(cr.ToString().ToDouble());
            if (!challengeRatings.Any())
                throw new Exception();

            var and = "%2C";
            var challenge_rating = "";
            foreach (var challengeRating in challengeRatings)
                challenge_rating += $"{challengeRating}{and}";
            var challenge_rating_truncated = challenge_rating.Substring(0, challenge_rating.Count() - and.Count()).Replace("0,", "0.");

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/monsters?challenge_rating={challenge_rating_truncated}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MonsterApiResponseDto>(result);
        }

        // Méthode pour récupérer un monstre en fonction de son index
        public async Task<Dnd5eApiMonsterDTO> GetMonsterByIndexAsync(string index)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://www.dnd5eapi.co/api/2014/monsters/{index}");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            // Désérialiser la réponse en un objet DTO spécifique au monstre
            return JsonConvert.DeserializeObject<Dnd5eApiMonsterDTO>(result);
        }

        private static List<string> GetCrUntil(double challengeRating)
            => GetAllCr().Where(cr => cr.ToDouble() <= challengeRating).ToList();

        private static List<string> GetAllCr() => new List<string>
            {
                "0",
                "0,125",
                "0,25",
                "0,5",
                "1",
                "2",
                "3",
                "4",
                "5",
                "6",
                "7",
                "8",
                "9",
                "10",
                "11",
                "12",
                "13",
                "14",
                "15",
                "16",
                "17",
                "19",
                "20",
                "21",
                "22",
                "23",
                "24",
                "30"
            };

    }
}
