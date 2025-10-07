using System.Text.RegularExpressions;

namespace TWP.Api.Application.Helpers
{
    public static class CrParserHelpers
    {
        public static string ExtractChallengeRating(string challengeRatingText)
        {
            if (string.IsNullOrWhiteSpace(challengeRatingText))
                return "0";

            // Pattern matches: "1/4", "1/2", "1", "10", "30", etc.
            var match = Regex.Match(challengeRatingText, @"^(\d+/\d+|\d+)");

            return match.Success ? match.Groups[1].Value : "0";
        }

        // For converting to decimal (useful for sorting/filtering)
        public static decimal ParseChallengeRating(string challengeRatingText)
        {
            var cr = ExtractChallengeRating(challengeRatingText);

            // Handle fractions
            if (cr.Contains('/'))
            {
                var parts = cr.Split('/');
                if (parts.Length == 2 &&
                    decimal.TryParse(parts[0], out var numerator) &&
                    decimal.TryParse(parts[1], out var denominator) &&
                    denominator != 0)
                {
                    return numerator / denominator;
                }
            }

            // Handle whole numbers
            if (decimal.TryParse(cr, out var result))
                return result;

            return 0;
        }
    }
}
