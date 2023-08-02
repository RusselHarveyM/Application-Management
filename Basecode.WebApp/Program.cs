using Microsoft.AspNetCore;

namespace Basecode.WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .ConfigureAppConfiguration(SetUpConfiguration)
            .UseStartup<StartupWebApp>();
    }

    private static void SetUpConfiguration(WebHostBuilderContext builderCtx, IConfigurationBuilder config)
    {
        config.Sources.Clear(); // Clears the default configuration options

        var env = builderCtx.HostingEnvironment;

        // Include settings file back to the configuration
        config.SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
    }
}