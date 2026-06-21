using System.Text.Json;
using CatalogService.Common.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }
    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ApiResponse<object>
        {
            status = false,
            Message = "An error occurred while processing your request.",
            Code = StatusCodes.Status500InternalServerError.ToString()
        };

        switch (ex)
        {
            case DbUpdateConcurrencyException:
                errorResponse.Message = "Data concurrency conflict occurred.";
                errorResponse.Code = StatusCodes.Status409Conflict.ToString();
                break;

            case DbUpdateException:
                errorResponse.Message = "Database error occurred.";
                errorResponse.Code = StatusCodes.Status500InternalServerError.ToString();
                break;

            case InvalidOperationException:
                errorResponse.Message = "Invalid operation requested.";
                errorResponse.Code = StatusCodes.Status400BadRequest.ToString();
                break;

            case UnauthorizedAccessException:
                errorResponse.Message = "You are not authorized to perform this action.";
                errorResponse.Code = StatusCodes.Status401Unauthorized.ToString();
                break;

            case KeyNotFoundException:
                errorResponse.Message = "Requested resource not found.";
                errorResponse.Code = StatusCodes.Status404NotFound.ToString();
                break;

            case SqlException sqlEx:
                errorResponse.Message = "Database connection error. Please try again later.";
                errorResponse.Code = StatusCodes.Status503ServiceUnavailable.ToString();
                _logger.LogError(sqlEx, "SQL Error: {Number}", sqlEx.Number);
                break;

            case HttpRequestException:
                errorResponse.Message = "Service communication error.";
                errorResponse.Code = StatusCodes.Status503ServiceUnavailable.ToString();
                break;

            default:
                if (_env.IsDevelopment())
                {
                    errorResponse.Message = ex.Message;
                    errorResponse.Data = ex.StackTrace;
                }
                break;
        }
        response.StatusCode = int.Parse(errorResponse.Code ?? StatusCodes.Status500InternalServerError.ToString());
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}