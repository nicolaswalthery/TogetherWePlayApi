using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IUltraModern5eBusinessLayer
    {
        public List<RollTableEntryDto> ShootAndLootGeneration();
    }
}
