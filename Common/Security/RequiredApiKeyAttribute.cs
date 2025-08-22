using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Security
{
    /// <summary>
    /// Attribute qui force la validation d'une API Key sur un controller ou une action
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiredApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Vérifier le header
            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var providedKey))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = "error",
                    message = "API Key is missing"
                });
                return;
            }

            // 2. Récupérer les options de configuration
            var options = context.HttpContext.RequestServices.GetRequiredService<ApiKeyOptions>();

            // 3. Vérifier d'abord la variable d'environnement
            var envKey = Environment.GetEnvironmentVariable("API_KEY");
            if (!string.IsNullOrEmpty(envKey) && envKey.Equals(providedKey.ToString()))
            {
                context.HttpContext.Items["ApiKey"] = providedKey.ToString();
                await next();
                return;
            }

            // 4. Vérifier dans la liste des API Keys configurées
            var isValid = options.ApiKeys.Any(k =>
                k.IsActive &&
                k.Key.Equals(providedKey.ToString()));

            if (!isValid)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = "error",
                    message = "Invalid API Key"
                });
                return;
            }

            context.HttpContext.Items["ApiKey"] = providedKey.ToString();
            await next();
        }
    }
}
