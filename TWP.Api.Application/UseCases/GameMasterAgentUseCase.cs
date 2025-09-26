using Common.ResultPattern;
using TWP.Api.Application.UseCases.Interfaces;

namespace TWP.Api.Application.UseCases
{
    public class GameMasterAgentUseCase : IGameMasterAgentUseCase
    {

        public GameMasterAgentUseCase()
        {
            
        }
        public Task<Result<string>> AskGameMasterAgent(string userPrompt)
        {
            throw new NotImplementedException();
        }
    }
}
