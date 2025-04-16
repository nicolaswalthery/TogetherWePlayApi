using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IPathfinder2eConditionsJsonRepository
    {
        public List<Pf2eConditionDto> GetAll();
    }
}
