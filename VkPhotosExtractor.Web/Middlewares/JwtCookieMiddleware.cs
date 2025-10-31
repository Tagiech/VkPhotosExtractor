namespace VkPhotosExtractor.Web.Middlewares;

public class JwtCookieMiddleware
{
    private readonly RequestDelegate _next;

    public JwtCookieMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            var jwt = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(jwt))
            {
                context.Request.Headers.Append("Authorization", $"Bearer {jwt}");
            }
        }

        await _next(context);
    }
}