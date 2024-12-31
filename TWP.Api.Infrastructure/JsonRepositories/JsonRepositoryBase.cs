using Common.Extensions;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class JsonRepositoryBase
    {
        private string _randomTablesRelativePath = @"TWP.Api.Infrastructure\JsonFiles\RandomTables";
        private string _baseRelativePath = @"TWP.Api.Infrastructure\JsonFiles";
        private SourceEnum _folderName;
        private readonly string _fileName;

        public JsonRepositoryBase()
        {
        }

        /// <summary>
        /// JsonRepositoryBase main Constructor
        /// </summary>
        /// <param name="folderName">Folder name where the random table json is located.</param>
        /// <remarks><param name="folderName"> is named after the ttrpg system where the random table is coming from.</remarks>
        public JsonRepositoryBase(SourceEnum folderName, string fileName)
        {
            _folderName = folderName;
            _fileName = fileName;
        }

        public JsonRepositoryBase(SourceEnum folderName)
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

    }
}
