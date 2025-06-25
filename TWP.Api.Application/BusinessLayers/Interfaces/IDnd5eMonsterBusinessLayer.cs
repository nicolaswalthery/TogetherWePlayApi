using Common.ResultPattern;
using System.Threading.Tasks;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDnd5eMonsterBusinessLayer
    {
        Task<Result<string>> GetAllMonsterStatsCsv();
    }
} 