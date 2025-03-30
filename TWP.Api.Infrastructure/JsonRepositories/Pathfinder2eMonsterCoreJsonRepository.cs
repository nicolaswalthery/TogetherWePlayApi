using Common.Extensions;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class Pathfinder2eMonsterCoreJsonRepository : JsonRepositoryBase, IPathfinder2eJsonRepository
    {
        public Pathfinder2eMonsterCoreJsonRepository() : base(folderName: SourceFolderEnum.Pathfinder2eMonsterCore)
        {
        }

        public List<Pf2eMonsterDto> GetAllPf2eCoreMonsters()
        {
            var results = new List<Pf2eMonsterDto>();
            var allFileNamesFromBaseFolder = base.GetAllJsonFileNamesInBaseFolder();
            foreach (var fileName in allFileNamesFromBaseFolder)
                results.Add(base.GetRollTable(fileName: fileName).FromJson<Pf2eMonsterDto>());

            return results;
        }


    }
}
