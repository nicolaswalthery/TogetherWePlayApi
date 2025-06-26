using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories.RepositoryBases
{
    public class Dnd5eDataJsonRepositoryBase : JsonRepositoryBase
    {
        private static string _parentFolderName = @"Dnd5eData";

        public Dnd5eDataJsonRepositoryBase(SourceFolderEnum folderName, string fileName)
            : base(parentFolderName: _parentFolderName, folderName, fileName)
        {
        }

        public Dnd5eDataJsonRepositoryBase(SourceFolderEnum folderName)
            : base(parentFolderName: _parentFolderName, folderName)
        {
        }
    }
}
