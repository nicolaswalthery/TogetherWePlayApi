using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Security
{
    /// <summary>
    /// Attribute qui force la validation d'une API Key sur un controller ou une action
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequiredApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        // Common/Security/RequiredApiKeyAttribute.cs
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // 1. Récupérer la clé fournie
            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var providedKey))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = "error",
                    message = "API Key missing"
                });
                return;
            }

            // 2. Récupérer LA clé valide (une seule source de vérité)
            var validKey =
                Environment.GetEnvironmentVariable("API_KEY") ??  // Priorité 1: Env
                context.HttpContext.RequestServices
                    .GetRequiredService<IConfiguration>()["Security:ApiKey"];  // Priorité 2: Config

            // 3. Valider
            if (string.IsNullOrEmpty(validKey) || !validKey.Equals(providedKey.ToString()))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    status = "error",
                    message = "Invalid API Key"
                });
                return;
            }

            // 4. Succès - continuer vers le controller
            context.HttpContext.Items["ApiKey"] = providedKey.ToString();
            await next();
        }
    }
}
