using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class RandomTableJsonRepositoryBase: JsonRepositoryBase
    {
        private static string _parentFolderName = @"RandomTables";
        public RandomTableJsonRepositoryBase(SourceFolderEnum folderName, string fileName) 
            : base(parentFolderName: _parentFolderName, folderName, fileName)
        {
        }

        public RandomTableJsonRepositoryBase(SourceFolderEnum folderName)
            : base(parentFolderName: _parentFolderName, folderName)
        {
        }

    }
}
