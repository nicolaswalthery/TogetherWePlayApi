using Common.ResultPattern;
using TWP.Api.Application.DataEnrichmentLayer.Interfaces;
using TWP.Api.Infrastructure.Interops.Interfaces;

namespace TWP.Api.Application.DataEnrichmentLayer
{
    public class Monster5eDataEnrichmentLayer : IMonster5eDataEnrichmentLayer
    {
        private readonly IOpenAiInterops _openAiInterops;

        public Monster5eDataEnrichmentLayer(IOpenAiInterops openAiInterops)
        {
            _openAiInterops = openAiInterops;
        }

        public async Task<Result> AiRoleDetermination()
           => await Safe.ExecuteAsync(async () =>
           {
               throw new NotImplementedException();
           });

        public async Task<Result> AiLoreDetermination()
           => await Safe.ExecuteAsync(async () =>
           {
               throw new NotImplementedException();
           });
    }
}
