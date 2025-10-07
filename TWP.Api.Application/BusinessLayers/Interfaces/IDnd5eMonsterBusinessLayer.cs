using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDnd5eMonsterBusinessLayer
    {
        Task<Result<List<Monster5eDto>>> GetAll5eMonsters();
        Task<Result<List<Dnd5eMonsterDto>>> GetAllMonsterStatsByCr(int cr);
        Task<Result<Monster5eDto>> CreateOriginalDndMonster(float challengeRating);
    }
} 