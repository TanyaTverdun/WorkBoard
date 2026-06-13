using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WorkBoard.Application.Common.Exceptions;

namespace WorkBoard.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "An error occurred during request execution: {Path}", 
                context.Request.Path);

            var problemDetails = new ValidationProblemDetails
            {
                Title = "An unexpected error occurred on the server.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            switch (ex)
            {
                case ValidationException valEx:
                    problemDetails.Title = "One or more validation errors occurred.";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Detail = "The request data failed validation checks.";

                    problemDetails.Errors = valEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    break;

                case NotFoundException notFoundEx:
                    problemDetails.Title = "The requested resource was not found.";
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Detail = notFoundEx.Message;
                    break;

                case ForbiddenAccessException forbiddenEx:
                    problemDetails.Title = "Access denied.";
                    problemDetails.Status = StatusCodes.Status403Forbidden;
                    problemDetails.Detail = forbiddenEx.Message;
                    break;

                case UnauthorizedAccessException:
                    problemDetails.Title = "Unauthorized access.";
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    problemDetails.Detail = 
                        "You must be authenticated or have correct " +
                        "permissions to access this resource.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = problemDetails.Status ?? 
                StatusCodes.Status500InternalServerError;

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
