using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Core.Interfaces.Infrastructure.JsonRepositories
{
    public interface IPathfinder2eConditionsJsonRepository
    {
        public List<Pf2eConditionDto> GetAll();
    }
}
