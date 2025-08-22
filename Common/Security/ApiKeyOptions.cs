namespace Common.Security
{
    /// <summary>
    /// Options de configuration pour l'API Key
    /// </summary>
    public class ApiKeyOptions
    {
        public const string SectionName = "Security";

        /// <summary>
        /// Nom du header HTTP contenant l'API Key
        /// </summary>
        public string HeaderName { get; set; } = "X-API-Key";

        /// <summary>
        /// Liste des API Keys valides (pour support multi-clients)
        /// </summary>
        public List<ApiKeyConfiguration> ApiKeys { get; set; } = new();

        /// <summary>
        /// Active le rate limiting
        /// </summary>
        public bool EnableRateLimiting { get; set; } = true;

        /// <summary>
        /// Nombre max de requêtes par minute
        /// </summary>
        public int MaxRequestsPerMinute { get; set; } = 100;

        /// <summary>
        /// Active les logs détaillés
        /// </summary>
        public bool EnableDetailedLogging { get; set; } = false;
    }

    public class ApiKeyConfiguration
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiresAt { get; set; }
        public List<string> AllowedIps { get; set; } = new();
    }
}
