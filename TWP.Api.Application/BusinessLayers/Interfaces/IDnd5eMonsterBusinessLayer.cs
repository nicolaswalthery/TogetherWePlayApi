using Common.ResultPattern;
using System.Threading.Tasks;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDnd5eMonsterBusinessLayer
    {
        Task<Result<List<Dnd5eMonsterDto>>> GetAllMonsterStatsCsv();
    }
} 