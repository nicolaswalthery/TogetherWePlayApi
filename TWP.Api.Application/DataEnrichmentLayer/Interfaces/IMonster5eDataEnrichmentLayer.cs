using Common.ResultPattern;

namespace TWP.Api.Application.DataEnrichmentLayer.Interfaces
{
    public interface IMonster5eDataEnrichmentLayer
    {
        Task<Result> AiRoleDetermination();
        Task<Result> AiLoreAndMannerDetermination();
    }
}
