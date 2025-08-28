using Common.ResultPattern;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDnd5eMonsterBusinessLayer
    {
        Task<Result<List<Monster5eDto>>> GetAll5eMonsters();
        Task<Result<List<Dnd5eMonsterDto>>> GetAllMonsterStatsByCr(int cr);
    }
} 