using TWP.Api.Core.DataTransferObjects;
using System.Threading.Tasks;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IPathfinder2eBusinessLayer
    {
        Task<Result<Pf2eMonsterDto>> GetOneRandomPf2eCoreMonster();
        Task<Result<Pf2eConditionDto>> GetOneRandomPf2eCondition();
    }
}
