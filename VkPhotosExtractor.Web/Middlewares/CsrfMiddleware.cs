namespace VkPhotosExtractor.Web.Middlewares;

public class CsrfMiddleware
{
    private readonly RequestDelegate _next;
    public CsrfMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsDelete(context.Request.Method))
        {
            var csrfToken = context.Request.Headers["X-CSRF-Token"].FirstOrDefault();
            var expectedCsrfToken = context.Request.Cookies["CSRF-Token"];

            if (string.IsNullOrWhiteSpace(csrfToken) ||
                string.IsNullOrWhiteSpace(expectedCsrfToken) ||
                !string.Equals(csrfToken, expectedCsrfToken, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("CSRF Token is invalid or missing.");
                return;
            }
        }

        await _next(context);
    }
}