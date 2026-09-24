using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Filters
{
    // Schützt schreibende Endpunkte (POST/PUT/DELETE) mit einem einfachen API-Key im Header,
    // statt sie komplett offen zu lassen. Erwarteter Wert kommt aus Configuration ("ApiKey") -
    // lokal via User Secrets, in Azure via App Settings, nie aus dem Repo.
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string HeaderName = "X-Api-Key";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var expectedKey = configuration["ApiKey"];

            if (string.IsNullOrEmpty(expectedKey))
            {
                context.Result = new ObjectResult("ApiKey ist auf dem Server nicht konfiguriert.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey)
                || providedKey != expectedKey)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }
    }
}
