namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class JsonRepositoryBase
    {
        private readonly string _folderName = "JsonFiles";
        public JsonRepositoryBase() { }

        protected string FolderName => _folderName;
    }
}
