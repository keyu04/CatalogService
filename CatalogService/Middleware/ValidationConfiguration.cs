using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Middlewares;

public static class ValidationConfiguration
{
    public static void UseCustomValidationResponse(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = new
                {
                    success    = false,
                    statusCode = 422,
                    message    = "Validation failed.",
                    errors     = errors
                };

                return new UnprocessableEntityObjectResult(response);
            };
        });
    }
}