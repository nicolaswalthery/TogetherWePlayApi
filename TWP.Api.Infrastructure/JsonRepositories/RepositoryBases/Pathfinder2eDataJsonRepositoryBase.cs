using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class Pathfinder2eDataJsonRepositoryBase : JsonRepositoryBase
    {
        public Pathfinder2eDataJsonRepositoryBase(SourceFolderEnum nestedfolderName) 
            : base(parentFolderName: @"Pathfinders2eData", nestedfolderName)
        {
        }
    }
}
