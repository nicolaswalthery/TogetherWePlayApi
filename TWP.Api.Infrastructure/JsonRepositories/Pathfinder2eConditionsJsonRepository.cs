using Common.Extensions;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class Pathfinder2eConditionsJsonRepository : Pathfinder2eDataJsonRepositoryBase, IPathfinder2eConditionsJsonRepository
    {
        public Pathfinder2eConditionsJsonRepository() : base(nestedfolderName: SourceFolderEnum.Pathfinder2eConditions)
        {
        }

        public List<Pf2eConditionDto> GetAll()
        {
            var results = new List<Pf2eConditionDto>();
            var allFileNamesFromBaseFolder = base.GetAllJsonFileNamesInBaseFolder();
            foreach (var fileName in allFileNamesFromBaseFolder)
                results.Add(base.GetJsonByFileName(fileName: fileName).JsonToObject<Pf2eConditionDto>());

            return results;
        }
    }
}
