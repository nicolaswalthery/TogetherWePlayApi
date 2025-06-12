using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace TogetherWePlayApi.Controllers
{
    public abstract class ControllerBase<T> : ControllerBase
    {
        protected readonly ILogger<T> Logger;

        protected ControllerBase(ILogger<T> logger)
        {
            Logger = logger;
        }

        protected IActionResult HandleResult<TData>(Result<TData> result)
        {
            if (result.IsSuccess)
                return Ok(new { status = "success", data = result.Data });

            Logger.LogWarning($"[FAILURE] [{result.ReasonType}] {result.Error}", result.ReasonType, result.Error);

            var errorPayload = new { status = "error", message = result.Error };

            return result.ReasonType switch
            {
                ReasonType.BadParameter or ReasonType.ValidationFailed =>
                    BadRequest(errorPayload),

                ReasonType.AlreadyExists or ReasonType.Conflict =>
                    Conflict(errorPayload),

                ReasonType.NotFound =>
                    NotFound(errorPayload),

                ReasonType.Unauthorized or ReasonType.InvalidCredentials or
                ReasonType.TokenInvalid or ReasonType.TokenExpired or ReasonType.SessionExpired =>
                    Unauthorized(errorPayload),

                ReasonType.Forbidden =>
                    Forbid(),

                ReasonType.NotImplemented =>
                    StatusCode(StatusCodes.Status501NotImplemented, errorPayload),

                ReasonType.Timeout or ReasonType.DependencyFailure or
                ReasonType.ExternalServiceError or ReasonType.DatabaseError or
                ReasonType.Failure or ReasonType.Unexpected =>
                    StatusCode(StatusCodes.Status500InternalServerError, errorPayload),

                _ => StatusCode(StatusCodes.Status500InternalServerError, errorPayload)
            };
        }

        protected IActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(new { status = "success" });

            Logger.LogWarning($"[FAILURE] [{result.ReasonType}] {result.Error}", result.ReasonType, result.Error);

            var errorPayload = new { status = "error", message = result.Error };

            return result.ReasonType switch
            {
                ReasonType.BadParameter or ReasonType.ValidationFailed =>
                    BadRequest(errorPayload),

                ReasonType.AlreadyExists or ReasonType.Conflict =>
                    Conflict(errorPayload),

                ReasonType.NotFound =>
                    NotFound(errorPayload),

                ReasonType.Unauthorized or ReasonType.InvalidCredentials or
                ReasonType.TokenInvalid or ReasonType.TokenExpired or ReasonType.SessionExpired =>
                    Unauthorized(errorPayload),

                ReasonType.Forbidden =>
                    Forbid(),

                ReasonType.NotImplemented =>
                    StatusCode(StatusCodes.Status501NotImplemented, errorPayload),

                ReasonType.Timeout or ReasonType.DependencyFailure or
                ReasonType.ExternalServiceError or ReasonType.DatabaseError or
                ReasonType.Failure or ReasonType.Unexpected =>
                    StatusCode(StatusCodes.Status500InternalServerError, errorPayload),

                _ => StatusCode(StatusCodes.Status500InternalServerError, errorPayload)
            };
        }
    }
}