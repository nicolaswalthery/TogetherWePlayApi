using Common.ResultPattern;

namespace TWP.Api.Application.UseCases.Interfaces
{
    public interface IGameMasterAgentUseCase
    {
        public Task<Result<string>> AskGameMasterAgent(string userPrompt);
    }
}
