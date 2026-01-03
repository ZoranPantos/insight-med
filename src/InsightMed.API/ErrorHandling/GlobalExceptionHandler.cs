using InsightMed.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly Dictionary<Type, Func<HttpContext, Exception, ProblemDetails>> _exceptionHandlers;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));

        _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, ProblemDetails>>
        {
            {
                typeof(ResourceNotFoundException),
                (ctx, ex) => HandleResourceNotFoundException(ctx, (ResourceNotFoundException)ex)
            },
            {
                typeof(InvalidClientDataException),
                (ctx, ex) => HandleInvalidClientDataException(ctx, (InvalidClientDataException)ex)
            },
            {
                typeof(UnauthorizedException),
                (ctx, ex) => HandleUnauthorizedException(ctx, (UnauthorizedException)ex)
            }
        };
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = _exceptionHandlers.TryGetValue(exception.GetType(), out var handler)
            ? handler.Invoke(httpContext, exception)
            : DefaultHandler(httpContext, exception);

        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        await _problemDetailsService.WriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problem,
                Exception = exception
            }
        );

        return true;
    }

    private ProblemDetails DefaultHandler(HttpContext ctx, Exception ex)
    {
        string logMessage = $"Internal Server Error: {ex.Message}";
        _logger.LogError(ex, logMessage);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        return problemDetails;
    }

    private ProblemDetails HandleResourceNotFoundException(HttpContext ctx, ResourceNotFoundException ex)
    {
        string logMessage = $"Resource Not Found: {ex.Message}";
        _logger.LogInformation(ex, logMessage);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        return problemDetails;
    }

    private ProblemDetails HandleInvalidClientDataException(HttpContext ctx, InvalidClientDataException ex)
    {
        string logMessage = $"Invalid Client Data: {ex.Message}";
        _logger.LogWarning(ex, logMessage);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        return problemDetails;
    }

    private ProblemDetails HandleUnauthorizedException(HttpContext ctx, UnauthorizedException ex)
    {
        string logMessage = $"Unauthorized access: {ex.Message}";
        _logger.LogWarning(ex, logMessage);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized Access",
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        return problemDetails;
    }
}