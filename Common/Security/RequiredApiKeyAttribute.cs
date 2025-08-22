using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Security
{
    /// <summary>
    /// Attribute qui force la validation d'une API Key sur un controller ou une action
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequiredApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _headerName;
        private readonly string _configKey;

        /// <summary>
        /// Create an Api Key Validation Attribute
        /// </summary>
        /// <param name="headerName">HTTP Header name</param>
        /// <param name="configKey">configuration for the Api Key</param>
        public RequiredApiKeyAttribute(string configKey, string headerName = "X-API-Key")
        {
            _headerName = headerName;
            _configKey = configKey;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(_headerName, out var providedKey))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = "error",
                    message = "API Key is missing",
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            var configuration = context.HttpContext.RequestServices
                .GetRequiredService<IConfiguration>();

            var validApiKey = Environment.GetEnvironmentVariable("API_KEY")
                ?? configuration[_configKey];

            if (string.IsNullOrEmpty(validApiKey))
            {
                context.Result = new ObjectResult(new
                {
                    status = "error",
                    message = "Server configuration error"
                })
                {
                    StatusCode = 500
                };
                return;
            }

            // 3. Valider l'API Key
            if (!validApiKey.Equals(providedKey.ToString()))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = "error",
                    message = "Invalid API Key",
                    timestamp = DateTime.UtcNow
                });
                return;
            }

            context.HttpContext.Items["ApiKey"] = providedKey.ToString();
            context.HttpContext.Items["ApiKeyValidatedAt"] = DateTime.UtcNow;

            await next();
        }
    }
}
