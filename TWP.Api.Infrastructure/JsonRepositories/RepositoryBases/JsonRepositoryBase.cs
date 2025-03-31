using Common.Extensions;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class JsonRepositoryBase
    {
        private string _randomTablesFolderRelativePath = @"TWP.Api.Infrastructure\JsonFiles\RandomTables";
        
        private string _baseRelativePath = @"TWP.Api.Infrastructure\JsonFiles";
        private SourceFolderEnum _childFolderName;
        private string _parentFolderName;
        private readonly string _fileName;
        
        public JsonRepositoryBase()
        {
        }

        public JsonRepositoryBase(string parentFolderName, SourceFolderEnum childFolderName, string fileName)
        {
            _parentFolderName = parentFolderName;
            _childFolderName = childFolderName;
            _fileName = fileName;
        }

        public JsonRepositoryBase(string parentFolderName, SourceFolderEnum childFolderName)
        {
            _parentFolderName = parentFolderName;
            _childFolderName = childFolderName;
        }

        /// <summary>
        /// JsonRepositoryBase main Constructor
        /// </summary>
        /// <param name="folderName">Folder name where the random table json is located.</param>
        /// <remarks><param name="folderName"> is named after the ttrpg system where the random table is coming from.</remarks>
        public JsonRepositoryBase(SourceFolderEnum folderName, string fileName)
        {
            _childFolderName = folderName;
            _fileName = fileName;
        }

        public JsonRepositoryBase(SourceFolderEnum folderName)
        {
            _childFolderName = folderName;
        }

        protected string RandomTablesRelativePath => _randomTablesFolderRelativePath;
        protected string BaseFolderRelativePath => _baseRelativePath; //TWP.Api.Infrastructure\JsonFiles
        protected string FullRandomTablesRelativePath => $@"{_randomTablesFolderRelativePath}\{_childFolderName}";
        protected string FullRelativePath => $@"{_baseRelativePath}\{_parentFolderName}\{_childFolderName}";

        protected string GetJsonByFileName()
            => _fileName.GetJsonFile(FullRelativePath);

        protected string GetJsonByFileName(string fileName)
            => fileName.GetJsonFile(FullRelativePath);

        /// <summary>
        /// Retrieves all JSON file names from a specified folder under the base JSON path.
        /// </summary>
        /// <returns>A list of JSON file names without their full path.</returns>
        protected List<string> GetAllJsonFileNamesInBaseFolder()
        {
            var results = new List<string>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\", FullRelativePath);
            if (!Directory.Exists(path))
                return results;

            results = Directory.GetFiles(path, "*.json")
                            .Select(Path.GetFileName)
                            .ToList();

            return results;
        }

    }
}
