using HtmlAgilityPack;
using System.Globalization;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.Interops.Interfaces;

namespace TWP.Api.Infrastructure.Interops
{
    public class AideDdInterops : IAideDdInterops
    {
        private readonly HttpClient _httpClient;

        public AideDdInterops(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AideDdMonsterResponseDto> GetMonsterByName(string monsterName)
        {
            monsterName = "knight"; // Temporary hardcoded for testing
            var url = $"https://www.aidedd.org/monster/{Uri.EscapeDataString(monsterName.Replace(" ", "-"))}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var html = await response.Content.ReadAsStringAsync();
            var result = Parse(html);
            return result;
        }

        public static AideDdMonsterResponseDto Parse(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var monster = new AideDdMonsterResponseDto
            {
                // Extraction du nom du monstre
                Name = doc.DocumentNode.SelectSingleNode("//h1")?.InnerText.Trim(),

                // Extraction du type et alignement
                Type = doc.DocumentNode.SelectSingleNode("//div[@class='type']")?.InnerText.Trim(),

                // Extraction du Challenge Rating (CR)
                ChallengeRating = doc.DocumentNode
                    .SelectSingleNode("//strong[contains(text(), 'CR')]/following-sibling::text()")
                    ?.InnerText.Trim() ?? "None",

                // Extraction du Challenge Rating Text
                ChallengeRatingText = doc.DocumentNode
                    .SelectSingleNode("//strong[contains(text(), 'CR')]/following-sibling::text()")
                    ?.InnerText.Trim() ?? "None",

                // Extraction de l'Armor Class (AC)
                ArmorClass = doc.DocumentNode.SelectSingleNode("//strong[text()='AC']/following-sibling::text()")?.InnerText.Trim(),

                // Extraction des Hit Points (HP)
                HitPoints = doc.DocumentNode.SelectSingleNode("//strong[text()='HP']/following-sibling::text()")?.InnerText.Trim(),

                // Extraction de la vitesse
                Speed = doc.DocumentNode.SelectSingleNode("//strong[text()='Speed']/following-sibling::text()")?.InnerText.Trim(),

                // Extraction des mots-clés de vitesse
                SpeedKeywords = doc.DocumentNode.SelectSingleNode("//strong[text()='Speed']/following-sibling::text()")?.InnerText.Trim(),

                // Extraction de la taille (Size) depuis la div .type
                Size = ExtractSize(doc.DocumentNode.SelectSingleNode("//div[@class='type']")?.InnerText),

                // Extraction de l'alignement
                Alignment = doc.DocumentNode.SelectSingleNode("//div[@class='type']")?.InnerText.Trim(),

                // Extraction de la légendaire
                Legendary = doc.DocumentNode.SelectSingleNode("//div[@class='legend']")?.InnerText.Contains("Legendary") ?? false,

                // Extraction de l'habitat
                Habitat = doc.DocumentNode.SelectSingleNode("//div[@class='habitat']")?.InnerText.Trim(),

                // Extraction de la source
                Source = doc.DocumentNode.SelectSingleNode("//div[@class='source']")?.InnerText.Trim(),

                // Extraction de l'image (si présente)
                HasImage = doc.DocumentNode.SelectSingleNode("//div[@class='picture']/img") != null,

                // Extraction des traductions (FR, ES, etc.)
                Translations = doc.DocumentNode.SelectNodes("//div[@class='trad']")
                    ?.Select(n => n.InnerText.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToList() ?? new List<string>(),

                // Extraction des Traits
                Traits = Extract(doc, "Traits"),

                // Extraction des Actions
                Actions = Extract(doc, "Actions"),

                // Extraction des Bonus Actions
                BonusActions = Extract(doc, "Bonus actions"),

                // Extraction des Legendary Actions
                LegendaryActions = Extract(doc, "Legendary actions"),

                ReactionActions = Extract(doc, "Reactions")
            };

            return monster;
        }

        // Extraction de la taille (Size)
        private static string ExtractSize(string typeText)
        {
            if (string.IsNullOrWhiteSpace(typeText)) return "";

            var sizeParts = typeText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return sizeParts.Length > 0 ? sizeParts[0] : "";
        }

        private static List<string> Extract(HtmlDocument doc, string title)
        {
            var abilities = new List<string>();

            // Trouver la section Actions
            var rubNode = doc.DocumentNode.SelectSingleNode($"//div[@class='rub' and text()='{title}']");

            if (rubNode != null)
            {
                var actionNodes = rubNode.SelectNodes("following-sibling::p | following-sibling::div[@class='rub']");

                
                if (actionNodes != null)
                {
                    var nextSectionFound = false;
                    for (int i = 0; i < actionNodes.Count && !nextSectionFound; i++)
                    {
                        var actionNode = actionNodes[i];
                        var actionText = actionNode.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(actionText) && !nextSectionFound)
                            abilities.Add(actionText);

                        if (i+1 != actionNodes.Count && actionNodes[i+1].Name == "div" && actionNodes[i + 1].GetAttributeValue("class", "") == "rub")
                            nextSectionFound = true;
                    }
                }
            }

            return abilities;
        }

    }
}
