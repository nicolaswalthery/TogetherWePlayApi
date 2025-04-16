using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IPathfinder2eBusinessLayer
    {
        public Pf2eMonsterDto GetOneRandomPf2eCoreMonster();
        public Pf2eConditionDto GetOneRandomPf2eCondition();
    }
}
