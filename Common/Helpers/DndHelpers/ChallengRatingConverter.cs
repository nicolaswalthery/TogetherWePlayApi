namespace Common.Helpers.DndHelpers
{
    public static class ChallengRatingConverter
    {
        public static double ConvertToDoubleChallengeRating(this string challengeRating)
        {
            // Handle integer values (e.g., "1", "2", etc.)
            if (double.TryParse(challengeRating, out double result))
            {
                return result;
            }

            // Handle fractional values (e.g., "1/8", "1/4", "1/2")
            if (challengeRating.Contains("/"))
            {
                var parts = challengeRating.Split('/');
                if (parts.Length == 2 && double.TryParse(parts[0], out double numerator) && double.TryParse(parts[1], out double denominator))
                {
                    return numerator / denominator;
                }
            }

            // Default return if parsing fails
            throw new ArgumentException("Invalid challenge rating format");
        }
    }
}
