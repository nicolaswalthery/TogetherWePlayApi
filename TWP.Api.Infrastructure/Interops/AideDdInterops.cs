using HtmlAgilityPack;
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
            monsterName = "balor"; // Temporary hardcoded for testing
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

                // Extraction du Challenge Rating (CR)
                ChallengeRating = Get(doc, "CR"),

                Gear = Get(doc, "Gear"),

                Senses = Get(doc, "Senses"),

                Languages = Get(doc, "Languages"),

                Skills = Get(doc, "Skills"),

                Resistances = Get(doc, "Resistances"),

                Immunities = Get(doc, "Immunities"),

                Initiative = Get(doc, "Initiative"),

                TypeAndSubtype = GetType(doc),

                // Extraction de l'Armor Class (AC)
                ArmorClass = doc.DocumentNode.SelectSingleNode("//strong[text()='AC']/following-sibling::text()")?.InnerText.Trim(),

                // Extraction des Hit Points (HP)
                HitPoints = doc.DocumentNode.SelectSingleNode("//strong[text()='HP']/following-sibling::text()")?.InnerText.Trim(),

                // Extraction de la vitesse
                Speed = doc.DocumentNode.SelectSingleNode("//strong[text()='Speed']/following-sibling::text()")?.InnerText.Trim(),

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

                ReactionActions = Extract(doc, "Reactions"),
            };

            var strData = GetPhysicalCaracteristicScoreModAnbdSave(doc, "Str");
            var dexData = GetPhysicalCaracteristicScoreModAnbdSave(doc, "Dex");
            var conData = GetPhysicalCaracteristicScoreModAnbdSave(doc, "Con");
            var intData = GetMentalCaracteristicScoreModAnbdSave(doc, "Int");
            var wisData = GetMentalCaracteristicScoreModAnbdSave(doc, "Wis");
            var chaData = GetMentalCaracteristicScoreModAnbdSave(doc, "Cha");

            monster.Strength = strData.score;
            monster.Dexterity = dexData.score;
            monster.Constitution = conData.score;
            monster.Intelligence = intData.score;
            monster.Wisdom = wisData.score;
            monster.Charisma = chaData.score;

            monster.StrengthMod = strData.mod;
            monster.DexterityMod = dexData.mod;
            monster.ConstitutionMod = conData.mod;
            monster.IntelligenceMod = intData.mod;
            monster.WisdomMod = wisData.mod;
            monster.CharismaMod = chaData.mod;

            monster.StrengthSave = strData.save;
            monster.DexteritySave = dexData.save;
            monster.ConstitutionSave = conData.save;
            monster.IntelligenceSave = intData.save;
            monster.WisdomSave = wisData.save;
            monster.CharismaSave = chaData.save;

            return monster;
        }

        private static string GetType(HtmlDocument doc)
        {
            var red = doc.DocumentNode
                                .SelectSingleNode("//div[contains(concat(' ', normalize-space(@class), ' '), ' red ')]");

            var typeNode = red?
                                .SelectSingleNode("./div[contains(concat(' ', normalize-space(@class), ' '), ' type ')]");

            return HtmlEntity.DeEntitize(typeNode?.InnerText ?? "").Trim();
        }

        private static string Get(HtmlDocument doc, string keyWord)
        {
            return doc.DocumentNode
                    .SelectSingleNode($"//strong[contains(text(), '{keyWord}')]/following-sibling::text()")
                    ?.InnerText.Trim() ?? "None";
        }

        private static CaracteristicData GetPhysicalCaracteristicScoreModAnbdSave(HtmlDocument doc, string carac)
        {
            var caracLabel = doc.DocumentNode.SelectSingleNode($"//div[@class='car1' and normalize-space(.)='{carac}']");
            var score = caracLabel?.SelectSingleNode("following-sibling::div[@class='car2'][1]")?.InnerText.Trim();
            var mod = caracLabel?.SelectSingleNode("following-sibling::div[@class='car3'][1]")?.InnerText.Trim();
            var save = caracLabel?.SelectSingleNode("following-sibling::div[@class='car3'][2]")?.InnerText.Trim();

            return new CaracteristicData(score, mod, save);
        }

        private static CaracteristicData GetMentalCaracteristicScoreModAnbdSave(HtmlDocument doc, string carac)
        {
            var caracLabel = doc.DocumentNode.SelectSingleNode($"//div[@class='car4' and normalize-space(.)='{carac}']");
            var score = caracLabel?.SelectSingleNode("following-sibling::div[@class='car5'][1]")?.InnerText.Trim();
            var mod = caracLabel?.SelectSingleNode("following-sibling::div[@class='car6'][1]")?.InnerText.Trim();
            var save = caracLabel?.SelectSingleNode("following-sibling::div[@class='car6'][2]")?.InnerText.Trim();

            return new CaracteristicData(score, mod, save);
        }

        private record CaracteristicData(string score, string mod, string save);

        private static void ExtractCharacteristicAndSaves(HtmlDocument doc, AideDdMonsterResponseDto monster)
        {
            //TODO : Extrairaire carac et saves https://www.aidedd.org/public/monster/tarrasque
            var statRows = doc.DocumentNode.SelectNodes("//div[contains(@class, 'car1')]/following-sibling::div");

            var strLabel = doc.DocumentNode.SelectSingleNode("//div[@class='car1' and normalize-space(.)='Str']");
            var strScore = strLabel?.SelectSingleNode("following-sibling::div[@class='car2'][1]")?.InnerText.Trim();
            var strMod = strLabel?.SelectSingleNode("following-sibling::div[@class='car3'][1]")?.InnerText.Trim();
            var strSave = strLabel?.SelectSingleNode("following-sibling::div[@class='car3'][2]")?.InnerText.Trim();

            if (statRows != null)
            {
                for (int i = 0; i < statRows.Count; i++)
                {
                    var statRow = statRows[i];
                    if (statRow.InnerText.Contains("Str") || statRow.InnerText.Contains("Dex") ||
                        statRow.InnerText.Contains("Con") || statRow.InnerText.Contains("Int") ||
                        statRow.InnerText.Contains("Wis") || statRow.InnerText.Contains("Cha"))
                    {
                        var statName = statRows[i - 1].InnerText.Trim();
                        var statValue = statRows[i + 1].InnerText.Trim();

                        // Assign to corresponding fields in the monster object
                        switch (statName)
                        {
                            case "Str":
                                monster.Strength = statValue;
                                break;
                            case "Dex":
                                monster.Dexterity = statValue;
                                break;
                            case "Con":
                                monster.Constitution = statValue;
                                break;
                            case "Int":
                                monster.Intelligence = statValue;
                                break;
                            case "Wis":
                                monster.Wisdom = statValue;
                                break;
                            case "Cha":
                                monster.Charisma = statValue;
                                break;
                        }
                    }
                }
            }
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
