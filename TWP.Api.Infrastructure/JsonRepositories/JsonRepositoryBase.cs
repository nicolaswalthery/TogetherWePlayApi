using Common.Extensions;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class JsonRepositoryBase
    {
        private string _randomTablesRelativePath = @"TWP.Api.Infrastructure\JsonFiles\RandomTables";
        private string _baseRelativePath = @"TWP.Api.Infrastructure\JsonFiles";
        private SourceFolderEnum _folderName;
        private readonly string _fileName;

        public JsonRepositoryBase()
        {
        }

        /// <summary>
        /// JsonRepositoryBase main Constructor
        /// </summary>
        /// <param name="folderName">Folder name where the random table json is located.</param>
        /// <remarks><param name="folderName"> is named after the ttrpg system where the random table is coming from.</remarks>
        public JsonRepositoryBase(SourceFolderEnum folderName, string fileName)
        {
            _folderName = folderName;
            _fileName = fileName;
        }

        public JsonRepositoryBase(SourceFolderEnum folderName)
        {
            _folderName = folderName;
        }

        protected string RandomTablesRelativePath => _randomTablesRelativePath;
        protected string BaseFolderRelativePath => _baseRelativePath;
        protected string FullRandomTablesRelativePath => $@"{_randomTablesRelativePath}\{_folderName}";

        protected string GetRollTable()
            => _fileName.GetJsonFile(FullRandomTablesRelativePath);

        protected string GetRollTable(string fileName)
            => fileName.GetJsonFile(FullRandomTablesRelativePath);

        /// <summary>
        /// Retrieves all JSON file names from a specified folder under the base JSON path.
        /// </summary>
        /// <returns>A list of JSON file names without their full path.</returns>
        protected List<string> GetAllJsonFileNamesInBaseFolder()
        {
            var folderPath = Path.Combine(BaseFolderRelativePath, _folderName.ToString());
            var results = new List<string>();
            if (!Directory.Exists(folderPath))
                return results;

            results = Directory.GetFiles(folderPath, "*.json")
                            .Select(Path.GetFileName)
                            .ToList();

            return results;
        }

    }
}
