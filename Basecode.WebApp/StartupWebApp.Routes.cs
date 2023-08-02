namespace Basecode.WebApp;

public partial class StartupWebApp
{
    private void ConfigureRoutes(IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                "token",
                "api/{token}");

            endpoints.MapRazorPages();
        });
    }
}