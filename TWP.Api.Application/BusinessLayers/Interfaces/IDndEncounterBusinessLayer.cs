using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDndEncounterBusinessLayer
    {
        RollTableEntryDto EncounterRandomGenerator();
    }
}
