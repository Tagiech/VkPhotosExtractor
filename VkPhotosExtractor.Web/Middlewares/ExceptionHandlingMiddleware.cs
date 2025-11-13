using VkPhotosExtractor.Application.Exceptions;

namespace VkPhotosExtractor.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = Guid.NewGuid().ToString("N");
        context.Response.ContentType = "application/json";

        _logger.LogInformation("""
                               Handling exception with
                               TraceId: {@TraceId}
                               Exception message: {@ExceptionMessage}
                               Stack trace: {@StackTrace}
                               """,
            traceId, ex.Message, ex.StackTrace);
        switch (ex)
        {
            case ApplicationException<InnerErrorCode> innerEx:
                await WriteErrorResponse(context, innerEx.StatusCode, innerEx.ErrorCode.ToString(),
                    $"Internal application error: {innerEx.Message}", traceId);
                break;
            case ApplicationException<ExternalErrorCode> externalEx:
                await WriteErrorResponse(context, externalEx.StatusCode, externalEx.ErrorCode.ToString(),
                    $"External application failed with error: {externalEx.Message}", traceId);
                break;
            default:
                await WriteErrorResponse(context, StatusCodes.Status500InternalServerError, 
                    "INTERNAL_ERROR", "Unexpected server error", traceId);
                break;
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string errorCode,
        string errorMessage, string traceId)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            error = errorCode,
            message = errorMessage,
            trace_id = traceId
        });
    }
}
