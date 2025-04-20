namespace VkPhotosExtractor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // builder.Services.AddAuthentication()
        //     .AddOpenIdConnect("oidc", options =>
        //     {
        //         options.UsePkce = true;
        //     });
        builder.Services.AddControllersWithViews();
        var app = builder.Build();

        //app.UseAuthentication();
        app.MapControllers();
        app.MapDefaultControllerRoute();
        

        app.Run();
    }
}