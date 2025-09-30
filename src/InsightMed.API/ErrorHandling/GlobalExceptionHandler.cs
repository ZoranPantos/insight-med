using InsightMed.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace InsightMed.API.ErrorHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    // TODO: Add logging exceptions

    private readonly IProblemDetailsService _problemDetailsService;
    private readonly Dictionary<Type, Func<HttpContext, Exception, ProblemDetails>> _exceptionHandlers;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;

        _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, ProblemDetails>>
        {
            {
                typeof(ResourceNotFoundException),
                (ctx, ex) => HandleResourceNotFoundException(ctx, (ResourceNotFoundException)ex)
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

        // TODO: log exception

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
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        return problemDetails;
    }

    private ProblemDetails HandleResourceNotFoundException(HttpContext ctx, ResourceNotFoundException ex)
    {
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Resource not found",
            Detail = ex.Message,
            Instance = ctx.Request.Path
        };

        return problemDetails;
    }
}
