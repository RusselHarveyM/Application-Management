namespace Basecode.WebApp;

public partial class StartupWebApp
{
    private void ConfigureCors(IServiceCollection services)
    {
        services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        }));
    }
}