using System.Net.Http;
using System.Threading.Tasks;
using TWP.Api.Infrastructure.Interops;
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

        public async Task<string> GetMonsterDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.dnd5eapi.co/api/2014/monsters");
            request.Headers.Add("Accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
