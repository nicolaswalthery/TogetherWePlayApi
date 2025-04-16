using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IPathfinder2eController
    {
        public Pf2eMonsterDto GetOneRandomCoreMonster();
        public Pf2eConditionDto GetOneRandomCondition();
    }
}
