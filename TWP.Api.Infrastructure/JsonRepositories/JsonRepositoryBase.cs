using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class JsonRepositoryBase
    {
        private string _randomTablesRelativePath = @"TWP.Api.Infrastructure\JsonFiles\RandomTables";
        private string _baseRelativePath = @"TWP.Api.Infrastructure\JsonFiles";
        private SourceEnum _folderName;

        public JsonRepositoryBase()
        {
        }

        public JsonRepositoryBase(SourceEnum folderName)
        {
            _folderName = folderName;
        }

        protected string RandomTablesRelativePath => _randomTablesRelativePath;
        protected string BaseFolderRelativePath => _baseRelativePath;

        protected string FullRandomTablesRelativePath => $@"{_randomTablesRelativePath}\{_folderName}";

    }
}
