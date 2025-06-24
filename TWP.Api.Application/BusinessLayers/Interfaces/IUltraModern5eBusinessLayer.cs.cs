using Common.ResultPattern;
using TWP.Api.Application.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IUltraModern5eBusinessLayer
    {
        Task<Result<ShootAndLootDto>> ShootAndLootGeneration();
    }
}
