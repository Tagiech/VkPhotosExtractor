using VkPhotosExtractor.Application.Exceptions;

namespace VkPhotosExtractor.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = Guid.NewGuid().ToString("N");
        context.Response.ContentType = "application/json";

        //TODO: log the exception with traceId
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
