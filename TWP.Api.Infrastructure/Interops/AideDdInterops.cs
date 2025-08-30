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
            monsterName = "tarrasque"; // Temporary hardcoded for testing
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
                Traits = ExtractTraits(doc),

                // Extraction des Actions
                Actions = ExtractActions(doc),

                // Extraction des Bonus Actions
                BonusActions = ExtractBonusActions(doc),

                // Extraction des Legendary Actions
                LegendaryActions = ExtractLegendaryActions(doc)
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

        // Extraction des Traits
        private static List<string> ExtractTraits(HtmlDocument doc)
        {
            var traitsList = new List<string>();

            // Trouver la section Traits
            var traitsNode = doc.DocumentNode.SelectSingleNode("//div[@class='rub' and text()='Traits']");

            if (traitsNode != null)
            {
                // Récupérer tous les paragraphes suivants jusqu'à la prochaine section
                var nextSectionNode = traitsNode.SelectSingleNode("following-sibling::div[@class='rub']");

                // Sélectionner les paragraphes entre Traits et la prochaine section
                var traitNodes = traitsNode
                    .SelectNodes("following-sibling::p[preceding-sibling::div[@class='rub'] != null or not(following-sibling::div[@class='rub'])]");

                if (traitNodes != null)
                {
                    foreach (var traitNode in traitNodes)
                    {
                        var traitText = traitNode.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(traitText))
                        {
                            traitsList.Add(traitText);
                        }
                    }
                }
            }

            return traitsList;
        }

        // Extraction des Actions
        private static List<string> ExtractActions(HtmlDocument doc)
        {
            var actionsList = new List<string>();

            // Trouver la section Actions
            var actionsNode = doc.DocumentNode.SelectSingleNode("//div[@class='rub' and text()='Actions']");

            if (actionsNode != null)
            {
                var actionNodes = actionsNode.SelectNodes("following-sibling::p");

                if (actionNodes != null)
                {
                    foreach (var actionNode in actionNodes)
                    {
                        var actionText = actionNode.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(actionText))
                        {
                            actionsList.Add(actionText);
                        }
                    }
                }
            }

            return actionsList;
        }

        // Extraction des Bonus Actions
        private static List<string> ExtractBonusActions(HtmlDocument doc)
        {
            var bonusActionsList = new List<string>();

            // Trouver la section Bonus actions
            var bonusActionsNode = doc.DocumentNode.SelectSingleNode("//div[@class='rub' and text()='Bonus actions']");

            if (bonusActionsNode != null)
            {
                var bonusActionNodes = bonusActionsNode.SelectNodes("following-sibling::p");

                if (bonusActionNodes != null)
                {
                    foreach (var bonusActionNode in bonusActionNodes)
                    {
                        var bonusActionText = bonusActionNode.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(bonusActionText))
                        {
                            bonusActionsList.Add(bonusActionText);
                        }
                    }
                }
            }

            return bonusActionsList;
        }

        // Extraction des Legendary Actions
        private static List<string> ExtractLegendaryActions(HtmlDocument doc)
        {
            var legendaryActionsList = new List<string>();

            // Trouver la section Legendary actions
            var legendaryActionsNode = doc.DocumentNode.SelectSingleNode("//div[@class='rub' and text()='Legendary actions']");

            if (legendaryActionsNode != null)
            {
                var legendaryActionNodes = legendaryActionsNode.SelectNodes("following-sibling::p");

                if (legendaryActionNodes != null)
                {
                    foreach (var legendaryActionNode in legendaryActionNodes)
                    {
                        var legendaryActionText = legendaryActionNode.InnerText.Trim();
                        if (!string.IsNullOrWhiteSpace(legendaryActionText))
                        {
                            legendaryActionsList.Add(legendaryActionText);
                        }
                    }
                }
            }

            return legendaryActionsList;
        }





    }
}
