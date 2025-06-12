using Common.ResultPattern;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IDndController
    {
        public Task<Result<RollTableEntryDto>> GetMonterActivity();
    }
}
