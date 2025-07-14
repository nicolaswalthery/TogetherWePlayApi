namespace TWP.Api.Infrastructure.CsvRepositories
{
    public class CsvRepositoryBase
    {
        private static string _baseRelativePath = @"TWP.Api.Infrastructure\CsvFiles";
        private readonly string _fileName;

        public CsvRepositoryBase()
        {   
        }

        public CsvRepositoryBase(string fileName)
        {
            _fileName = fileName;
        }

        /// <summary>
        /// Get the TWP.Api.Infrastructure\CsvFiles base folder path
        /// </summary>
        protected string BaseFolderRelativePath => _baseRelativePath;
        protected string FullRelativePath => $@"{_baseRelativePath}\";

        /// <summary>
        /// Retrieves the contents of the CSV file as a string.
        /// </summary>
        /// <returns>CSV file contents as a string.</returns>
        protected string GetCsvByFileName()
            => File.ReadAllText(GetCsvPath());

        protected string GetCsvPath()
        {
            // Remonte jusqu'au projet racine
            var basePath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;

            // Combine avec le dossier CSV
            var csvPath = Path.Combine(basePath, "TWP.Api.Infrastructure", "CsvFiles", _fileName);

            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"CSV file not found: {csvPath}");

            return csvPath;
        }
    }
} 