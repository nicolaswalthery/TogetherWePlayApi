using Common.ResultPattern;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDndEncounterBusinessLayer
    {
        Task<Result<RollTableEntryDto>> EncounterRandomGenerator();
    }
}
