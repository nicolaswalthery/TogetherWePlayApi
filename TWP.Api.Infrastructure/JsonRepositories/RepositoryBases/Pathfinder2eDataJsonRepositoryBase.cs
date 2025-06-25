using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class Pathfinder2eDataJsonRepositoryBase : JsonRepositoryBase
    {
        public Pathfinder2eDataJsonRepositoryBase(SourceFolderEnum folderName) 
            : base(parentFolderName: @"Pathfinders2eData", folderName)
        {
        }
    }
}
