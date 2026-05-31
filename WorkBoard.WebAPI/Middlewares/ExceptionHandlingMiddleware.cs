using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WorkBoard.WebAPI.Middlewares;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="ExceptionHandlingMiddleware"/> class
    /// </summary>
    /// <param name="next">
    /// The next delegate in the HTTP request pipeline
    /// </param>
    /// <param name="logger">
    /// The logger used for capturing error details
    /// </param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to process the incoming HTTP request
    /// </summary>
    /// <param name="context">
    /// The current <see cref="HttpContext"/> for the request
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation
    /// </returns>
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
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }
}
