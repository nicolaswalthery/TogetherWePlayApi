using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDndEncounterBusinessLayer
    {
        /// <summary>
        /// Generate Randomly an encounter
        /// </summary>
        /// <returns></returns>
        RollTableDto EncounterRandomGenerator();
    }
}
